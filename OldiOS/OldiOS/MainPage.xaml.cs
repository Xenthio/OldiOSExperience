using Microsoft.AspNetCore.Components.WebView;

namespace OldiOS
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			
			// Prevent external URL navigation in BlazorWebView
			blazorWebView.UrlLoading += OnUrlLoading;
		}
		
		private void OnUrlLoading(object? sender, UrlLoadingEventArgs e)
		{
			// Only allow navigation within the app's own domain
			// Block all external URLs from opening in system browser
			if (e.Url.Host != "0.0.0.0" && e.Url.Host != "localhost")
			{
				// Cancel external navigation - keep it in the iframe
				e.UrlLoadingStrategy = UrlLoadingStrategy.CancelLoad;
			}
		}
	}
}
