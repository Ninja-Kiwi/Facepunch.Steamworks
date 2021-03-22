using System;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamHTMLSurface : SteamClientClass<SteamHTMLSurface>
	{
		public static event Action<string> OnRequestStarted;
		public static event Action<string> OnRequestFinished;
		public static event Action<string> OnJSAlert;
		public static event Action<string> OnJSConfirm;
		public static event Action<uint, uint, byte[]> OnNeedsPaint;

		public static bool IsValid => Internal.IsValid;
		public static unsafe bool Init() => Internal.Init();
		public static unsafe bool Shutdown() => Internal.Shutdown();

		private static HHTMLBrowser browserHandle;

		internal static ISteamHTMLSurface Internal => Interface as ISteamHTMLSurface;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamHTMLSurface(server));
			InstallEvents();
		}

		internal static void InstallEvents()
		{
			Dispatch.Install<HTML_StartRequest_t>(x => OnRequestStarted?.Invoke(x.PchURL));
			Dispatch.Install<HTML_FinishedRequest_t>(x => OnRequestFinished?.Invoke(x.PchURL));
			Dispatch.Install<HTML_JSAlert_t>(x => OnJSAlert?.Invoke(x.PchMessage));
			Dispatch.Install<HTML_JSConfirm_t>(x => OnJSConfirm?.Invoke(x.PchMessage));
			Dispatch.Install<HTML_NeedsPaint_t>(x => PaintCallback(x.PBGRA, x.UnWide, x.UnTall));

			// Steamworks docs says "You must have implemented callback handlers" for the ones below even though 
			// HTML_BrowserReady_t is never triggered, and we don't need HTML_FileOpenDialog_t
			// https://partner.steamgames.com/doc/api/ISteamHTMLSurface
			Dispatch.Install<HTML_BrowserReady_t>(x => { });
			Dispatch.Install<HTML_FileOpenDialog_t>(x => { });
		}

		/// <summary>
		/// Throws System.InvalidOperationException @ htmlBrowser.Value when htmlBrowser.HasValue == false
		/// </summary>
		public static async Task CreateBrowser(string userAgent, string userCSS)
		{
			CallResult<HTML_BrowserReady_t> browserCall = Internal.CreateBrowser(userAgent, userCSS);
			HTML_BrowserReady_t? htmlBrowser = await browserCall.GetAwaiter();
			browserHandle = htmlBrowser.Value.UnBrowserHandle;
		}

		public static void LoadURL(string url, string postData = null)
		{
			Internal.LoadURL(browserHandle, url, postData);
		}

		public static void LoadURL(string url, uint width, uint height)
		{
			Internal.LoadURL(browserHandle, url, string.Empty);
			Internal.SetSize(browserHandle, width, height);
			Internal.SetKeyFocus(browserHandle, true);
		}

		private static byte[] imgBytes;
		private static void PaintCallback(IntPtr pBGRA, uint wide, uint tall)
		{
			var size = (int)(wide * tall * 4);
			if (imgBytes?.Length != size)
			{
				imgBytes = new byte[size];
			}
			System.Runtime.InteropServices.Marshal.Copy(pBGRA, imgBytes, 0, size);
			OnNeedsPaint?.Invoke(wide, tall, imgBytes);
		}

		public static void SetSize(uint unWidth, uint unHeight)
		{
			Internal.SetSize(browserHandle, unWidth, unHeight);
		}

		public static void Reload()
		{
			Internal.Reload(browserHandle);
		}

		public static void StopLoad()
		{
			Internal.StopLoad(browserHandle);
		}

		public static void MouseDown(HTMLMouseButton eMouseButton)
		{
			Internal.MouseDown(browserHandle, new IntPtr((int)eMouseButton));
		}

		public static void MouseWheel(int delta)
		{
			Internal.MouseWheel(browserHandle, delta);
		}

		public static void MouseUp(HTMLMouseButton eMouseButton)
		{
			Internal.MouseUp(browserHandle, new IntPtr((int)eMouseButton));
		}

		public static void KeyDown(uint nNativeKeyCode, HTMLKeyModifiers eHTMLKeyModifiers, bool bIsSystemKey = false)
		{
			Internal.KeyDown(browserHandle, nNativeKeyCode, new IntPtr((int)eHTMLKeyModifiers), bIsSystemKey);
		}

		public static void MouseMove(int x, int y)
		{
			Internal.MouseMove(browserHandle, x, y);
		}

		public static void KeyUp(uint nNativeKeyCode, HTMLKeyModifiers eHTMLKeyModifiers)
		{
			Internal.KeyUp(browserHandle, nNativeKeyCode, new IntPtr((int)eHTMLKeyModifiers));
		}

		public static void KeyChar(uint cUnicodeChar, HTMLKeyModifiers eHTMLKeyModifiers)
		{
			Internal.KeyChar(browserHandle, cUnicodeChar, new IntPtr((int)eHTMLKeyModifiers));
		}

		public static void SetVerticalScroll(uint nAbsolutePixelScroll)
		{
			Internal.SetVerticalScroll(browserHandle, nAbsolutePixelScroll);
		}

		public static void SetHorizontalScroll(uint nAbsolutePixelScroll)
		{
			Internal.SetHorizontalScroll(browserHandle, nAbsolutePixelScroll);
		}

		public static void AllowStartRequest(bool allow)
		{
			Internal.AllowStartRequest(browserHandle, allow);
		}

		public static void ExecuteJavascript(string script)
		{
			Internal.ExecuteJavascript(browserHandle, script);
		}

		public static void RemoveBrowser()
		{
			Internal.RemoveBrowser(browserHandle);
		}

		public static void GoBack()
		{
			Internal.GoBack(browserHandle);
		}

		public static void GoFormat()
		{
			Internal.GoForward(browserHandle);
		}
	}

	public enum HTMLMouseButton : int
	{
		Left = 0,
		Right = 1,
		Middle = 2,
	}

	public enum HTMLKeyModifiers : int
	{
		None = 0,
		AltDown = 1,
		CtrlDown = 2,
		ShiftDown = 4,
	}
}