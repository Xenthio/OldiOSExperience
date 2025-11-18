#if WINDOWS
using Microsoft.Web.WebView2.Core;
using OldiOS.Shared.Services;

namespace OldiOS.Services;

/// <summary>
/// Windows Safari service implementation using WebView2
/// </summary>
public class SafariService : ISafariService
{
    private Microsoft.UI.Xaml.Controls.WebView2? _webView;
    private string _currentUrl = "";
    private bool _isInitialized = false;
    
    // iPhone 4 / iOS 5 User Agent
    private const string iPhone4UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3";
    
    public event EventHandler<string>? NavigationCompleted;
    public event EventHandler<string>? NavigationFailed;
    
    public bool CanGoBack => _webView?.CanGoBack ?? false;
    public bool CanGoForward => _webView?.CanGoForward ?? false;
    
    public SafariService()
    {
    }
    
    public async Task InitializeWebView()
    {
        _webView = new Microsoft.UI.Xaml.Controls.WebView2();
        
        // Wait for CoreWebView2 initialization
        await _webView.EnsureCoreWebView2Async();
        
        if (_webView.CoreWebView2 != null)
        {
            // Set custom user agent
            _webView.CoreWebView2.Settings.UserAgent = iPhone4UserAgent;
            
            // Subscribe to navigation events
            _webView.CoreWebView2.NavigationCompleted += OnNavigationCompleted;
            _webView.CoreWebView2.NavigationStarting += OnNavigationStarting;
            
            _isInitialized = true;
        }
    }
    
    public Microsoft.UI.Xaml.Controls.WebView2? GetWebView() => _webView;
    
    public async Task<bool> LoadUrl(string url)
    {
        if (_webView == null || !_isInitialized || string.IsNullOrWhiteSpace(url))
            return false;
            
        try
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
            
            _currentUrl = url;
            _webView.Source = new Uri(url);
            
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Safari LoadUrl error: {ex.Message}");
            NavigationFailed?.Invoke(this, ex.Message);
            return false;
        }
    }
    
    public async Task<bool> NavigateBack()
    {
        if (_webView?.CanGoBack == true)
        {
            _webView.GoBack();
            return await Task.FromResult(true);
        }
        return false;
    }
    
    public async Task<bool> NavigateForward()
    {
        if (_webView?.CanGoForward == true)
        {
            _webView.GoForward();
            return await Task.FromResult(true);
        }
        return false;
    }
    
    public async Task Reload()
    {
        _webView?.Reload();
        await Task.CompletedTask;
    }
    
    public string GetCurrentUrl()
    {
        return _webView?.Source?.ToString() ?? _currentUrl;
    }
    
    private void OnNavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
    {
        if (args.IsSuccess)
        {
            _currentUrl = sender.Source;
            NavigationCompleted?.Invoke(this, _currentUrl);
        }
        else
        {
            NavigationFailed?.Invoke(this, $"Navigation failed with error code: {args.WebErrorStatus}");
        }
    }
    
    private void OnNavigationStarting(CoreWebView2 sender, CoreWebView2NavigationStartingEventArgs args)
    {
        _currentUrl = args.Uri;
    }
}
#endif
