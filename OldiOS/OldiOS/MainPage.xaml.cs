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
}
