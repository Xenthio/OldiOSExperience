using Microsoft.AspNetCore.Components.WebView;

namespace OldiOS
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			
			// Configure BlazorWebView to allow loading local files
			blazorWebView.BlazorWebViewInitializing += OnBlazorWebViewInitializing;
			blazorWebView.UrlLoading += OnUrlLoading;
		}

		private void OnBlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e)
		{
			// Additional web view configuration can be added here if needed
#if WINDOWS
			// Windows-specific configuration
			e.WebViewConfiguration.UserAgent = "OldiOS/1.0";
#elif IOS || MACCATALYST
			// iOS/Mac-specific configuration
#elif ANDROID
			// Android-specific configuration
			if (e.WebView != null)
			{
				e.WebView.Settings.AllowFileAccess = true;
				e.WebView.Settings.AllowFileAccessFromFileURLs = true;
				e.WebView.Settings.AllowUniversalAccessFromFileURLs = true;
			}
#endif
		}

		private void OnUrlLoading(object? sender, UrlLoadingEventArgs e)
		{
			// Allow all app:// URLs (Blazor's internal protocol)
			if (e.Url.Scheme == "app")
			{
				return;
			}

			// Allow file:// URLs for local album artwork
			if (e.Url.Scheme == "file")
			{
				return;
			}

			// Allow http/https for external resources
			if (e.Url.Scheme == "http" || e.Url.Scheme == "https")
			{
				return;
			}

			// Block all other protocols for security
			e.UrlLoadingStrategy = UrlLoadingStrategy.CancelLoad;
		}
	}
}
