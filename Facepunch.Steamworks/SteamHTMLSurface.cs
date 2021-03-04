using System;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
    public class SteamHTMLSurface : SteamClientClass<SteamHTMLSurface>
	{
		internal static ISteamHTMLSurface Internal => Interface as ISteamHTMLSurface;

		internal override void InitializeInterface( bool server )
		{
			SetInterface( server, new ISteamRemoteStorage( server ) );
			InstallEvents();
		}

		internal static void InstallEvents()
		{
			Dispatch.Install<HTML_BrowserReady_t>(x => OnBrowserReady?.Invoke());
			Dispatch.Install<HTML_StartRequest_t>(x => OnRequestStarted?.Invoke(x.PchURL));
			Dispatch.Install<HTML_FinishedRequest_t>(x => OnRequestFinished?.Invoke(x.PchURL));
			Dispatch.Install<HTML_JSAlert_t>(x => OnJSAlert?.Invoke());
			Dispatch.Install<HTML_JSConfirm_t>(x => OnJSConfirm?.Invoke());
			Dispatch.Install<HTML_FileOpenDialog_t>(x => OnFileOpenDialog?.Invoke());
			Dispatch.Install<HTML_NeedsPaint_t>(x => OnNeedsPaint?.Invoke(x.UnWide, x.UnTall));
		}

		public static event Action OnBrowserReady; // Paul: this didn't work on the old facepunch 0.7
		public static event Action<string> OnRequestStarted;
		public static event Action<string> OnRequestFinished;
		public static event Action OnJSAlert;
		public static event Action OnJSConfirm;
		public static event Action OnFileOpenDialog;
		public static event Action<uint, uint> OnNeedsPaint;

		private HHTMLBrowser browserHandle;

		public bool IsValid => Internal.IsValid;

		public unsafe bool Init() => Internal.Init();

		public unsafe bool Shutdown() => Internal.Shutdown();

		/// <summary>
		/// Throws System.InvalidOperationException @ htmlBrowser.Value when htmlBrowser.HasValue == false
		/// </summary>
		public async Task CreateBrowser(string pchUserAgent, string pchUserCSS)
		{
            CallResult<HTML_BrowserReady_t> browserCall = Internal.CreateBrowser(pchUserAgent, pchUserCSS);
            HTML_BrowserReady_t? htmlBrowser = await browserCall.GetAwaiter();
			browserHandle = htmlBrowser.Value.UnBrowserHandle;
		}

		public void LoadURL(string url, string pchPostData = null)
        {
			Internal.LoadURL(browserHandle, url, pchPostData);
        }

		public void LoadURL(string url, uint width, uint height)
		{
			Internal.LoadURL(browserHandle, url, string.Empty);
			Internal.SetSize(browserHandle, width, height);
			Internal.SetKeyFocus(browserHandle, true);
		}

		public void SetSize(uint unWidth, uint unHeight)
		{
			Internal.SetSize(browserHandle, unWidth, unHeight);
		}

		public void Reload()
		{
			Internal.Reload(browserHandle);
		}

		public void StopLoad()
		{
			Internal.StopLoad(browserHandle);
		}

		public void MouseDown(HTMLMouseButton eMouseButton)
		{
			Internal.MouseDown(browserHandle, new IntPtr((int)eMouseButton));
		}

		public void MouseWheel(int delta)
		{
			Internal.MouseWheel(browserHandle, delta);
		}

		public void MouseUp(HTMLMouseButton eMouseButton)
		{
			Internal.MouseUp(browserHandle, new IntPtr((int)eMouseButton));
		}

		public void KeyDown(uint nNativeKeyCode, HTMLKeyModifiers eHTMLKeyModifiers, bool bIsSystemKey = false)
		{
			Internal.KeyDown(browserHandle, nNativeKeyCode, new IntPtr((int)eHTMLKeyModifiers), bIsSystemKey);
		}

		public void MouseMove(int x, int y)
		{
			Internal.MouseMove(browserHandle, x, y);
		}

		public void KeyUp(uint nNativeKeyCode, HTMLKeyModifiers eHTMLKeyModifiers)
		{
			Internal.KeyUp(browserHandle, nNativeKeyCode, new IntPtr((int)eHTMLKeyModifiers));
		}

		public void KeyChar(uint cUnicodeChar, HTMLKeyModifiers eHTMLKeyModifiers)
		{
			Internal.KeyChar(browserHandle, cUnicodeChar, new IntPtr((int)eHTMLKeyModifiers));
		}

		public void SetVerticalScroll(uint nAbsolutePixelScroll)
		{
			Internal.SetVerticalScroll(browserHandle, nAbsolutePixelScroll);
		}

		public void SetHorizontalScroll(uint nAbsolutePixelScroll)
		{
			Internal.SetHorizontalScroll(browserHandle, nAbsolutePixelScroll);
		}

		public void AllowStartRequest(bool allow)
		{
			Internal.AllowStartRequest(browserHandle, allow);
		}

		public void ExecuteJavascript(string script)
		{
			Internal.ExecuteJavascript(browserHandle, script);
		}

		public void RemoveBrowser()
		{
			Internal.RemoveBrowser(browserHandle);
		}

		public void GoBack()
		{
			Internal.GoBack(browserHandle);
		}

		public void GoFormat()
		{
			Internal.GoForward(browserHandle);
		}
	}

	// ----------------------------------------------------------------

	//
	// ISteamHTMLSurface::EHTMLMouseButton
	//
	public enum HTMLMouseButton : int
	{
		Left = 0,
		Right = 1,
		Middle = 2,
	}

	//
	// ISteamHTMLSurface::EHTMLKeyModifiers
	//
	public enum HTMLKeyModifiers : int
	{
		None = 0,
		AltDown = 1,
		CtrlDown = 2,
		ShiftDown = 4,
	}
}