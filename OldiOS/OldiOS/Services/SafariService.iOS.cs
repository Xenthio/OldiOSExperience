#if IOS || MACCATALYST
using Foundation;
using OldiOS.Shared.Services;
using WebKit;

namespace OldiOS.Services;

/// <summary>
/// iOS/macOS Safari service implementation using WKWebView
/// </summary>
public class SafariService : NSObject, ISafariService, IWKNavigationDelegate
{
    private WKWebView? _webView;
    private string _currentUrl = "";
    
    // iPhone 4 / iOS 5 User Agent
    private const string iPhone4UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3";
    
    public event EventHandler<string>? NavigationCompleted;
    public event EventHandler<string>? NavigationFailed;
    
    public bool CanGoBack => _webView?.CanGoBack ?? false;
    public bool CanGoForward => _webView?.CanGoForward ?? false;
    
    public SafariService()
    {
        InitializeWebView();
    }
    
    private void InitializeWebView()
    {
        var config = new WKWebViewConfiguration();
        _webView = new WKWebView(CoreGraphics.CGRect.Empty, config)
        {
            NavigationDelegate = this,
            CustomUserAgent = iPhone4UserAgent
        };
    }
    
    public WKWebView? GetWebView() => _webView;
    
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
            var nsUrl = new NSUrl(url);
            var request = new NSUrlRequest(nsUrl);
            _webView.LoadRequest(request);
            
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
        return _webView?.Url?.AbsoluteString ?? _currentUrl;
    }
    
    // WKNavigationDelegate methods
    [Export("webView:didFinishNavigation:")]
    public void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
    {
        _currentUrl = webView.Url?.AbsoluteString ?? _currentUrl;
        NavigationCompleted?.Invoke(this, _currentUrl);
    }
    
    [Export("webView:didFailNavigation:withError:")]
    public void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
    {
        NavigationFailed?.Invoke(this, error.LocalizedDescription);
    }
    
    [Export("webView:didFailProvisionalNavigation:withError:")]
    public void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
    {
        NavigationFailed?.Invoke(this, error.LocalizedDescription);
    }
}
#endif
