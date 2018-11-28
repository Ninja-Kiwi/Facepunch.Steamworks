using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		public delegate void FailureCallback();
		
		public delegate void BrowserCallback();
		public delegate void StartRequestCallback(string pchURL);
		public delegate void NeedsPaintCallback(uint unWide, uint unTall, string pBGRA);
		public delegate void FinishedRequestCallback(string url);
		
		// public BrowserCallback OnBrowserReady;
		public NeedsPaintCallback OnNeedsPaint;
		public StartRequestCallback OnStartRequest; 
		public FinishedRequestCallback OnFinishedRequest; 

		internal HtmlSurface(Client c) {
			client = c;

			// can get from CreateBrowser()
			//client.RegisterCallback<SteamNative.HTML_BrowserReady_t>(OnBrowserReadyAPI);
			client.RegisterCallback<SteamNative.HTML_StartRequest_t>(OnStartRequestAPI);
			client.RegisterCallback<SteamNative.HTML_FinishedRequest_t>(OnFinishedRequestAPI);
			client.RegisterCallback<SteamNative.HTML_JSAlert_t>(OnJSAlertAPI);
			client.RegisterCallback<SteamNative.HTML_JSConfirm_t>(OnJSConfirmAPI);
			client.RegisterCallback<SteamNative.HTML_FileOpenDialog_t>(OnFileOpenDialogAPI);
			client.RegisterCallback<SteamNative.HTML_NeedsPaint_t>(OnNeedsPaintAPI);
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
		public void CreateBrowser(string pchUserAgent, string pchUserCSS, BrowserCallback onSuccess, FailureCallback onFailure = null) {
			client.native.htmlSurface.CreateBrowser(pchUserAgent, pchUserCSS, (result, error) => {
				if (error) {
					onFailure?.Invoke();
				}
				else {
					browserHandle = result.UnBrowserHandle;
					onSuccess();
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
		public void MouseDown(Facepunch.Steamworks.HTMLMouseButton eMouseButton ) {
			client.native.htmlSurface.MouseDown(browserHandle, (SteamNative.HTMLMouseButton) eMouseButton);
		}
		public void MouseWheel(int delta) {
			client.native.htmlSurface.MouseWheel(browserHandle, delta);
		}
		public void MouseUp(Facepunch.Steamworks.HTMLMouseButton eMouseButton) {
			client.native.htmlSurface.MouseUp(browserHandle, (SteamNative.HTMLMouseButton) eMouseButton);
		}
		public void KeyDown(uint nNativeKeyCode, Facepunch.Steamworks.HTMLKeyModifiers eHTMLKeyModifiers) {
			client.native.htmlSurface.KeyDown(browserHandle, nNativeKeyCode, (SteamNative.HTMLKeyModifiers) eHTMLKeyModifiers);
		}
		public void MouseMove(int x, int y) {
			client.native.htmlSurface.MouseMove(browserHandle, x, y);
		}
		public void KeyUp(uint nNativeKeyCode, Facepunch.Steamworks.HTMLKeyModifiers eHTMLKeyModifiers) {
			client.native.htmlSurface.KeyUp(browserHandle, nNativeKeyCode, (SteamNative.HTMLKeyModifiers)eHTMLKeyModifiers);
		}
		public void KeyChar(uint cUnicodeChar, Facepunch.Steamworks.HTMLKeyModifiers eHTMLKeyModifiers) {
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
		/*private unsafe void OnBrowserReadyAPI(HTML_BrowserReady_t callbackdata) {
			//Console.Error.WriteLine("HtmlSurface: OnBrowserReadyAPI");
			OnBrowserReady?.Invoke((uint)callbackdata.UnBrowserHandle);
		}*/
		private unsafe void OnStartRequestAPI(HTML_StartRequest_t callbackdata) {
			// doesn't show in unity? Need to add to SteamNative.Structs.cs e.g check HTML_FinishedRequest_t
			//throw new Exception("OnStartRequestAPI");
			OnStartRequest?.Invoke(callbackdata.PchURL);
			// client.native.htmlSurface.AllowStartRequest(callbackdata.UnBrowserHandle, true);
		}
		private unsafe void OnFinishedRequestAPI(HTML_FinishedRequest_t callbackdata) {
			// shows in unity. throw new Exception("OnFinishedRequestAPI");
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
			OnNeedsPaint?.Invoke(callbackdata.UnWide, callbackdata.UnTall, callbackdata.PBGRA);
			//Console.Error.WriteLine("HtmlSurface: OnFileOpenDialogAPI");
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