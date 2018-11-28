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

	// same as HHTMLBrowser, just public
	// only used to keep methods similar to steamworks.net
	// should never need more than one browser, probably ok to move into HtmlSurface
	public class HTMLBrowser {
		public uint Value;

		public static implicit operator HTMLBrowser(uint value) {
			return new HTMLBrowser() { Value = value };
		}

		public static implicit operator uint(HTMLBrowser value) {
			return value.Value;
		}
	}

	public class HtmlSurface {
		internal Client client;
		//internal HHTMLBrowser browserHandle;
		public delegate void FailureCallback();
		
		public delegate void BrowserCallback(HTMLBrowser browserHandle);
		public delegate void StartRequestCallback(HTMLBrowser browserHandle, string pchURL);
		public delegate void NeedsPaintCallback(uint unWide, uint unTall, string pBGRA);
		public delegate void FinishedRequestCallback(HTMLBrowser browserHandle, string url);
		
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
					// browserHandle = result.UnBrowserHandle;
					onSuccess((uint)result.UnBrowserHandle);
				}
			});
		}
		public void LoadURL(HTMLBrowser browserHandle, string url, string pchPostData = null) {
			client.native.htmlSurface.LoadURL((uint)browserHandle, url, pchPostData);
		}
		public void LoadURL(HTMLBrowser browserHandle, string url, uint width, uint height) {
			client.native.htmlSurface.LoadURL((uint)browserHandle, url, "");
			client.native.htmlSurface.SetSize((uint)browserHandle, width, height);
			client.native.htmlSurface.SetKeyFocus((uint)browserHandle, true);
		}
		public void SetSize(HTMLBrowser browserHandle, uint unWidth, uint unHeight) {
			client.native.htmlSurface.SetSize((uint)browserHandle, unWidth, unHeight);
		}
		public void Reload(HTMLBrowser browserHandle) {
			client.native.htmlSurface.Reload((uint)browserHandle);
		}
		public void StopLoad(HTMLBrowser browserHandle) {
			client.native.htmlSurface.StopLoad((uint)browserHandle);
		}
		public void MouseDown(HTMLBrowser browserHandle, Facepunch.Steamworks.HTMLMouseButton eMouseButton ) {
			client.native.htmlSurface.MouseDown((uint)browserHandle, (SteamNative.HTMLMouseButton) eMouseButton);
		}
		public void MouseWheel(HTMLBrowser browserHandle, int delta) {
			client.native.htmlSurface.MouseWheel((uint)browserHandle, delta);
		}
		public void MouseUp(HTMLBrowser browserHandle, Facepunch.Steamworks.HTMLMouseButton eMouseButton) {
			client.native.htmlSurface.MouseUp((uint)browserHandle, (SteamNative.HTMLMouseButton) eMouseButton);
		}
		public void KeyDown(HTMLBrowser browserHandle, uint nNativeKeyCode, Facepunch.Steamworks.HTMLKeyModifiers eHTMLKeyModifiers) {
			client.native.htmlSurface.KeyDown((uint)browserHandle, nNativeKeyCode, (SteamNative.HTMLKeyModifiers) eHTMLKeyModifiers);
		}
		public void MouseMove(HTMLBrowser browserHandle, int x, int y) {
			client.native.htmlSurface.MouseMove((uint)browserHandle, x, y);
		}
		public void KeyUp(HTMLBrowser browserHandle, uint nNativeKeyCode, Facepunch.Steamworks.HTMLKeyModifiers eHTMLKeyModifiers) {
			client.native.htmlSurface.KeyUp((uint)browserHandle, nNativeKeyCode, (SteamNative.HTMLKeyModifiers)eHTMLKeyModifiers);
		}
		public void KeyChar(HTMLBrowser browserHandle, uint cUnicodeChar, Facepunch.Steamworks.HTMLKeyModifiers eHTMLKeyModifiers) {
			client.native.htmlSurface.KeyChar((uint)browserHandle, cUnicodeChar, (SteamNative.HTMLKeyModifiers)eHTMLKeyModifiers);
		}
		public void SetVerticalScroll(HTMLBrowser browserHandle, uint nAbsolutePixelScroll) {
			client.native.htmlSurface.SetVerticalScroll((uint)browserHandle, nAbsolutePixelScroll);
		}
		public void SetHorizontalScroll(HTMLBrowser browserHandle, uint nAbsolutePixelScroll) {
			client.native.htmlSurface.SetHorizontalScroll((uint)browserHandle, nAbsolutePixelScroll);
		}
		public void AllowStartRequest(HTMLBrowser browserHandle, bool allow) {
			client.native.htmlSurface.AllowStartRequest((uint)browserHandle, allow);
		}
		public void ExecuteJavascript(HTMLBrowser browserHandle, string script) {
			client.native.htmlSurface.ExecuteJavascript((uint)browserHandle, script);
		}
		public void RemoveBrowser(HTMLBrowser browserHandle) {
			client.native.htmlSurface.RemoveBrowser((uint)browserHandle);
		}
		/*private unsafe void OnBrowserReadyAPI(HTML_BrowserReady_t callbackdata) {
			//Console.Error.WriteLine("HtmlSurface: OnBrowserReadyAPI");
			OnBrowserReady?.Invoke((uint)callbackdata.UnBrowserHandle);
		}*/
		private unsafe void OnStartRequestAPI(HTML_StartRequest_t callbackdata) {
			// doesn't show in unity? Need to add to SteamNative.Structs.cs e.g check HTML_FinishedRequest_t
			//throw new Exception("OnStartRequestAPI");
			OnStartRequest?.Invoke((uint)callbackdata.UnBrowserHandle, callbackdata.PchURL);
			// client.native.htmlSurface.AllowStartRequest(callbackdata.UnBrowserHandle, true);
		}
		private unsafe void OnFinishedRequestAPI(HTML_FinishedRequest_t callbackdata) {
			// shows in unity. throw new Exception("OnFinishedRequestAPI");
			OnFinishedRequest?.Invoke((uint)callbackdata.UnBrowserHandle, callbackdata.PchURL);
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