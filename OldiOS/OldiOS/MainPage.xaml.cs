using Microsoft.AspNetCore.Components.WebView;

namespace OldiOS
{
	public partial class MainPage : ContentPage
	{
		private const string iPhone4UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3";
		private Timer? _hashMonitorTimer;
		private string? _lastHash;
		
		public MainPage()
		{
			InitializeComponent();
			
			// Setup Safari WebView
			SetupSafariWebView();
			
			// Monitor for hash changes (Blazor to MAUI communication)
			StartHashMonitoring();
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
		
		private void StartHashMonitoring()
		{
			// Poll for hash changes to receive commands from Blazor
			_hashMonitorTimer = new Timer(async _ => await CheckHashForCommands(), null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
		}
		
		private async Task CheckHashForCommands()
		{
			try
			{
				var result = await blazorWebView.EvaluateJavaScriptAsync("window.location.hash");
				var hash = result?.ToString() ?? "";
				
				if (!string.IsNullOrEmpty(hash) && hash != _lastHash && hash.StartsWith("#safari-"))
				{
					_lastHash = hash;
					await ProcessSafariCommand(hash);
				}
			}
			catch
			{
				// Ignore errors during hash polling
			}
		}
		
		private async Task ProcessSafariCommand(string hash)
		{
			// Clear the hash after processing
			try
			{
				await blazorWebView.EvaluateJavaScriptAsync("window.location.hash = ''");
			}
			catch { }
			
			await MainThread.InvokeOnMainThreadAsync(() =>
			{
				if (hash.StartsWith("#safari-nav:"))
				{
					var url = Uri.UnescapeDataString(hash.Substring("#safari-nav:".Length));
					NavigateToUrl(url);
				}
				else if (hash == "#safari-back")
				{
					if (safariWebView.CanGoBack)
						safariWebView.GoBack();
				}
				else if (hash == "#safari-forward")
				{
					if (safariWebView.CanGoForward)
						safariWebView.GoForward();
				}
				else if (hash == "#safari-reload")
				{
					safariWebView.Reload();
				}
				else if (hash == "#safari-hide")
				{
					safariWebView.IsVisible = false;
				}
			});
		}
		
		private void NavigateToUrl(string url)
		{
			// Normalize URL
			if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
				!url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
			{
				if (url.Contains(".") && !url.Contains(" "))
				{
					url = "https://" + url;
				}
				else
				{
					url = "https://www.google.com/search?q=" + Uri.EscapeDataString(url);
				}
			}
			
			safariWebView.Source = url;
			safariWebView.IsVisible = true;
		}
		
		private void OnSafariNavigating(object? sender, WebNavigatingEventArgs e)
		{
			Console.WriteLine($"Safari navigating to: {e.Url}");
		}
		
		private void OnSafariNavigated(object? sender, WebNavigatedEventArgs e)
		{
			Console.WriteLine($"Safari navigated to: {e.Url}, Result: {e.Result}");
			
			// Notify Blazor of navigation completion
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				try
				{
					var escapedUrl = e.Url.Replace("'", "\\'");
					await blazorWebView.EvaluateJavaScriptAsync($"if(window.safariNativeInterop){{window.safariNativeInterop.onNavigated('{escapedUrl}');}}");
				}
				catch { }
			});
		}
	}
}
