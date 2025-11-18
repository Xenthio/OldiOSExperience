using Microsoft.AspNetCore.Components.WebView;

namespace OldiOS
{
	public partial class MainPage : ContentPage
	{
		private const string iPhone4UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3";
		
		public MainPage()
		{
			InitializeComponent();
			
			// Setup Safari WebView
			SetupSafariWebView();
			
			// Register as the Safari WebView controller
			if (Application.Current?.Handler?.MauiContext?.Services != null)
			{
				var service = Application.Current.Handler.MauiContext.Services.GetService(typeof(Services.ISafariWebViewService)) as Services.SafariWebViewService;
				if (service != null)
				{
					service.SetWebView(safariWebView);
				}
			}
		}
		
		private void SetupSafariWebView()
		{
#if IOS || MACCATALYST
			// Set custom user agent for iOS/macOS
			safariWebView.HandlerChanged += (s, e) =>
			{
				if (safariWebView.Handler?.PlatformView is WebKit.WKWebView wv)
				{
					wv.CustomUserAgent = iPhone4UserAgent;
					
					// IMPORTANT: Create a custom navigation delegate to handle all navigation in-app
					wv.NavigationDelegate = new SafariNavigationDelegate();
				}
			};
#elif ANDROID
			// Set custom user agent for Android
			safariWebView.HandlerChanged += (s, e) =>
			{
				if (safariWebView.Handler?.PlatformView is Android.Webkit.WebView wv)
				{
					wv.Settings.JavaScriptEnabled = true;
					wv.Settings.DomStorageEnabled = true;
					wv.Settings.UserAgentString = iPhone4UserAgent;
					
					// IMPORTANT: Set WebViewClient to prevent external browser launches
					wv.SetWebViewClient(new SafariWebViewClient());
				}
			};
#endif

			// Handle navigation events
			safariWebView.Navigated += OnSafariNavigated;
			safariWebView.Navigating += OnSafariNavigating;
		}
		
		private void OnSafariNavigating(object? sender, WebNavigatingEventArgs e)
		{
			Console.WriteLine($"Safari navigating to: {e.Url}");
			
			// CRITICAL: Cancel navigation if it's trying to open externally
			// Keep all navigation within the WebView
			if (e.NavigationEvent == WebNavigationEvent.NewPage)
			{
				// Allow navigation within the WebView
				e.Cancel = false;
			}
		}
		
		private void OnSafariNavigated(object? sender, WebNavigatedEventArgs e)
		{
			Console.WriteLine($"Safari navigated to: {e.Url}, Result: {e.Result}");
			
			// Notify service of navigation completion
			if (Application.Current?.Handler?.MauiContext?.Services != null)
			{
				var service = Application.Current.Handler.MauiContext.Services.GetService(typeof(Services.ISafariWebViewService)) as Services.SafariWebViewService;
				service?.OnNavigated(e.Url);
			}
		}
	}

#if IOS || MACCATALYST
	// Custom navigation delegate for iOS to keep all navigation in-app
	internal class SafariNavigationDelegate : WebKit.WKNavigationDelegate
	{
		public override void DecidePolicy(WebKit.WKWebView webView, WebKit.WKNavigationAction navigationAction, Action<WebKit.WKNavigationActionPolicy> decisionHandler)
		{
			// Allow all navigation within the WebView (don't open external Safari)
			decisionHandler(WebKit.WKNavigationActionPolicy.Allow);
		}
	}
#elif ANDROID
	// Custom WebViewClient for Android to keep all navigation in-app
	internal class SafariWebViewClient : Android.Webkit.WebViewClient
	{
		public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView? view, Android.Webkit.IWebResourceRequest? request)
		{
			// Return false to let WebView handle the URL (don't open external browser)
			if (view != null && request?.Url != null)
			{
				view.LoadUrl(request.Url.ToString());
			}
			return true; // We handled it
		}
	}
#endif
}
