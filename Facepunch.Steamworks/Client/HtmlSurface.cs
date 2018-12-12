using System;
using SteamNative;

namespace Facepunch.Steamworks {
	public partial class Client : IDisposable {
		HtmlSurface _htmlSurface;
		public HtmlSurface HtmlSurface
		{
			get
			{
				if (_htmlSurface == null)
					_htmlSurface = new HtmlSurface(this);
				return _htmlSurface;
			}
		}
	}

	public class HtmlSurface {
		internal Client client;
		internal HHTMLBrowser browserHandle;
		byte[] imgBytes;

		public delegate void FailureCallback();
		public delegate void BrowserCallback();
		public delegate void StartRequestCallback(string pchURL);
		public delegate void NeedsPaintCallback(uint unWide, uint unTall, byte[] imgBytes);
		public delegate void FinishedRequestCallback(string url);

		// Note: CreateBrowser() doesn't always seem to return when the browser is ready
		// use callback instead
		//public BrowserCallback OnBrowserReady;
		public NeedsPaintCallback OnNeedsPaint;
		public StartRequestCallback OnStartRequest; 
		public FinishedRequestCallback OnFinishedRequest; 

		internal HtmlSurface(Client c) {
			client = c;

			// can get from CreateBrowser()
			//client.RegisterCallback<HTML_BrowserReady_t>(OnBrowserReadyAPI); not getting callback
			client.RegisterCallback<HTML_StartRequest_t>(OnStartRequestAPI);
			client.RegisterCallback<HTML_FinishedRequest_t>(OnFinishedRequestAPI);
			client.RegisterCallback<HTML_JSAlert_t>(OnJSAlertAPI);
			client.RegisterCallback<HTML_JSConfirm_t>(OnJSConfirmAPI);
			client.RegisterCallback<HTML_FileOpenDialog_t>(OnFileOpenDialogAPI);
			client.RegisterCallback<HTML_NeedsPaint_t>(OnNeedsPaintAPI);
		}
		public bool IsValid() {
			return client.native.htmlSurface.IsValid;
		}
		public unsafe bool Init() {
			return client.native.htmlSurface.Init();
		}
		public void Dispose() {
			client = null;
		}
		public void CreateBrowser(string pchUserAgent, string pchUserCSS, BrowserCallback onSuccess, FailureCallback onFailure) {
			 
			client.native.htmlSurface.CreateBrowser(pchUserAgent, pchUserCSS, (result, error) => {
				if (error) {
					onFailure?.Invoke();
				}
				else {
					browserHandle = result.UnBrowserHandle;
					onSuccess?.Invoke();
				}
			});
		}
		public void LoadURL(string url, string pchPostData = null) {
			client.native.htmlSurface.LoadURL(browserHandle, url, pchPostData);
		}
		public void LoadURL(string url, uint width, uint height) {
			client.native.htmlSurface.LoadURL(browserHandle, url, "");
			client.native.htmlSurface.SetSize(browserHandle, width, height);
			client.native.htmlSurface.SetKeyFocus(browserHandle, true);
		}
		public void SetSize(uint unWidth, uint unHeight) {
			client.native.htmlSurface.SetSize(browserHandle, unWidth, unHeight);
		}
		public void Reload() {
			client.native.htmlSurface.Reload(browserHandle);
		}
		public void StopLoad() {
			client.native.htmlSurface.StopLoad(browserHandle);
		}
		public void MouseDown(HTMLMouseButton eMouseButton ) {
			client.native.htmlSurface.MouseDown(browserHandle, (SteamNative.HTMLMouseButton) eMouseButton);
		}
		public void MouseWheel(int delta) {
			client.native.htmlSurface.MouseWheel(browserHandle, delta);
		}
		public void MouseUp(HTMLMouseButton eMouseButton) {
			client.native.htmlSurface.MouseUp(browserHandle, (SteamNative.HTMLMouseButton) eMouseButton);
		}
		public void KeyDown(uint nNativeKeyCode, HTMLKeyModifiers eHTMLKeyModifiers) {
			client.native.htmlSurface.KeyDown(browserHandle, nNativeKeyCode, (SteamNative.HTMLKeyModifiers) eHTMLKeyModifiers);
		}
		public void MouseMove(int x, int y) {
			client.native.htmlSurface.MouseMove(browserHandle, x, y);
		}
		public void KeyUp(uint nNativeKeyCode, HTMLKeyModifiers eHTMLKeyModifiers) {
			client.native.htmlSurface.KeyUp(browserHandle, nNativeKeyCode, (SteamNative.HTMLKeyModifiers)eHTMLKeyModifiers);
		}
		public void KeyChar(uint cUnicodeChar, HTMLKeyModifiers eHTMLKeyModifiers) {
			client.native.htmlSurface.KeyChar(browserHandle, cUnicodeChar, (SteamNative.HTMLKeyModifiers)eHTMLKeyModifiers);
		}
		public void SetVerticalScroll(uint nAbsolutePixelScroll) {
			client.native.htmlSurface.SetVerticalScroll(browserHandle, nAbsolutePixelScroll);
		}
		public void SetHorizontalScroll(uint nAbsolutePixelScroll) {
			client.native.htmlSurface.SetHorizontalScroll(browserHandle, nAbsolutePixelScroll);
		}
		public void AllowStartRequest(bool allow) {
			client.native.htmlSurface.AllowStartRequest(browserHandle, allow);
		}
		public void ExecuteJavascript(string script) {
			client.native.htmlSurface.ExecuteJavascript(browserHandle, script);
		}
		public void RemoveBrowser() {
			client.native.htmlSurface.RemoveBrowser(browserHandle);
		}
		public void GoBack() {
		       client.native.htmlSurface.GoBack(browserHandle);
		}
		public void GoFormat() {
		       client.native.htmlSurface.GoForward(browserHandle);
		}
		//private unsafe void OnBrowserReadyAPI(HTML_BrowserReady_t callbackdata) {
		//	OnBrowserReady?.Invoke();
		//}
		private unsafe void OnStartRequestAPI(HTML_StartRequest_t callbackdata) {
			OnStartRequest?.Invoke(callbackdata.PchURL);
			// client.native.htmlSurface.AllowStartRequest(callbackdata.UnBrowserHandle, true);
		}
		private unsafe void OnFinishedRequestAPI(HTML_FinishedRequest_t callbackdata) {
			OnFinishedRequest?.Invoke(callbackdata.PchURL);
		}
		private unsafe void OnJSAlertAPI(HTML_JSAlert_t callbackdata) {
			Console.Error.WriteLine("HtmlSurface: OnJSAlertAPI");
		}
		private unsafe void OnJSConfirmAPI(HTML_JSConfirm_t callbackdata) {
			Console.Error.WriteLine("HtmlSurface: OnJSConfirmAPI");
		}
		private unsafe void OnFileOpenDialogAPI(HTML_FileOpenDialog_t callbackdata) {
			Console.Error.WriteLine("HtmlSurface: OnFileOpenDialogAPI");
		}
		private unsafe void OnNeedsPaintAPI(HTML_NeedsPaint_t callbackdata) {
			int dataSize = (int)(callbackdata.UnWide * callbackdata.UnTall * 4);
			if (imgBytes == null || imgBytes.Length != dataSize) {
				imgBytes = new byte[dataSize];
			}
			System.Runtime.InteropServices.Marshal.Copy(callbackdata.PBGRA, imgBytes, 0, dataSize);
			OnNeedsPaint?.Invoke(callbackdata.UnWide, callbackdata.UnTall, imgBytes);
		}
	}
	

	// ----------------------------------------------------------------

	// from SteamNative.Enums.cs
	// only added to avoid exposing SteamNative

	//
	// ISteamHTMLSurface::EHTMLMouseButton
	//
	public enum HTMLMouseButton : int {
		Left = 0,
		Right = 1,
		Middle = 2,
	}

	//
	// ISteamHTMLSurface::EHTMLKeyModifiers
	//
	public enum HTMLKeyModifiers : int {
		None = 0,
		AltDown = 1,
		CtrlDown = 2,
		ShiftDown = 4,
	}

}