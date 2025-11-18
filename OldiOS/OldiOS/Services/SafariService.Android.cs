#if ANDROID
using Android.Webkit;
using OldiOS.Shared.Services;

namespace OldiOS.Services;

/// <summary>
/// Android Safari service implementation using WebView
/// </summary>
public class SafariService : ISafariService
{
    private WebView? _webView;
    private string _currentUrl = "";
    
    // iPhone 4 / iOS 5 User Agent
    private const string iPhone4UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3";
    
    public event EventHandler<string>? NavigationCompleted;
    public event EventHandler<string>? NavigationFailed;
    
    public bool CanGoBack => _webView?.CanGoBack() ?? false;
    public bool CanGoForward => _webView?.CanGoForward() ?? false;
    
    public SafariService()
    {
    }
    
    public void InitializeWebView(Android.Content.Context context)
    {
        _webView = new WebView(context);
        _webView.Settings.JavaScriptEnabled = true;
        _webView.Settings.DomStorageEnabled = true;
        _webView.Settings.UserAgentString = iPhone4UserAgent;
        
        _webView.SetWebViewClient(new SafariWebViewClient(this));
    }
    
    public WebView? GetWebView() => _webView;
    
    public async Task<bool> LoadUrl(string url)
    {
        if (_webView == null || string.IsNullOrWhiteSpace(url))
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
            _webView.LoadUrl(url);
            
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
        if (_webView?.CanGoBack() == true)
        {
            _webView.GoBack();
            return await Task.FromResult(true);
        }
        return false;
    }
    
    public async Task<bool> NavigateForward()
    {
        if (_webView?.CanGoForward() == true)
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
        return _webView?.Url ?? _currentUrl;
    }
    
    private class SafariWebViewClient : WebViewClient
    {
        private readonly SafariService _safariService;
        
        public SafariWebViewClient(SafariService safariService)
        {
            _safariService = safariService;
        }
        
        public override void OnPageFinished(WebView? view, string? url)
        {
            base.OnPageFinished(view, url);
            if (!string.IsNullOrEmpty(url))
            {
                _safariService._currentUrl = url;
                _safariService.NavigationCompleted?.Invoke(_safariService, url);
            }
        }
        
        public override void OnReceivedError(WebView? view, IWebResourceRequest? request, WebResourceError? error)
        {
            base.OnReceivedError(view, request, error);
            _safariService.NavigationFailed?.Invoke(_safariService, error?.Description ?? "Unknown error");
        }
    }
}
#endif
