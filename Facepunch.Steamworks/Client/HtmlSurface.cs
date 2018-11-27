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

        public delegate void CreateBrowserCallback();
		public delegate void FailureCallback();

		internal HtmlSurface(Client c) {
			client = c;

            client.RegisterCallback<SteamNative.HTML_BrowserReady_t>(OnBrowserReadyAPI);
            client.RegisterCallback<SteamNative.HTML_StartRequest_t>(OnStartRequestAPI);
            client.RegisterCallback<SteamNative.HTML_JSAlert_t>(OnJSAlertAPI);
            client.RegisterCallback<SteamNative.HTML_JSConfirm_t>(OnJSConfirmAPI);
            client.RegisterCallback<SteamNative.HTML_FileOpenDialog_t>(OnFileOpenDialogAPI);
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

        public void CreateBrowser(string pchUserAgent, string pchUserCSS, CreateBrowserCallback onSuccess, FailureCallback onFailure = null) {
			client.native.htmlSurface.CreateBrowser(pchUserAgent, pchUserCSS, (result, error) => {
				if (error) {
					onFailure?.Invoke();
				} else {
                    browserHandle = result.UnBrowserHandle;
                    onSuccess();
                }
			});
		}

        public void LoadURL( string url, uint width, uint height )
        {
            client.native.htmlSurface.LoadURL(browserHandle, url, "");
            client.native.htmlSurface.SetSize(browserHandle, width, height);
            client.native.htmlSurface.SetKeyFocus(browserHandle, true);
        }

        private unsafe void OnBrowserReadyAPI(HTML_BrowserReady_t callbackdata)
        {
            Console.Error.WriteLine("HtmlSurface: OnBrowserReadyAPI");
        }
        private unsafe void OnStartRequestAPI(HTML_StartRequest_t callbackdata)
        {
            client.native.htmlSurface.AllowStartRequest(callbackdata.UnBrowserHandle, true);
        }
        private unsafe void OnJSAlertAPI(HTML_JSAlert_t callbackdata)
        {
            Console.Error.WriteLine("HtmlSurface: OnJSAlertAPI");
        }
        private unsafe void OnJSConfirmAPI(HTML_JSConfirm_t callbackdata)
        {
            Console.Error.WriteLine("HtmlSurface: OnJSConfirmAPI");
        }
        private unsafe void OnFileOpenDialogAPI(HTML_FileOpenDialog_t callbackdata)
        {
            Console.Error.WriteLine("HtmlSurface: OnFileOpenDialogAPI");
        }

    }

}