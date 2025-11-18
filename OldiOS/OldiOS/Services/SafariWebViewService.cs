namespace OldiOS.Services;

/// <summary>
/// MAUI platform implementation - controls the native WebView
/// </summary>
public class SafariWebViewService : ISafariWebViewService
{
    private WebView? _webView;
    
    public event EventHandler<string>? Navigated;

    public void SetWebView(WebView webView)
    {
        _webView = webView;
    }

    public void NavigateToUrl(string url)
    {
        if (_webView == null) return;

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

        MainThread.BeginInvokeOnMainThread(() =>
        {
            _webView.Source = url;
            _webView.IsVisible = true;
        });
    }

    public void GoBack()
    {
        if (_webView?.CanGoBack == true)
        {
            MainThread.BeginInvokeOnMainThread(() => _webView.GoBack());
        }
    }

    public void GoForward()
    {
        if (_webView?.CanGoForward == true)
        {
            MainThread.BeginInvokeOnMainThread(() => _webView.GoForward());
        }
    }

    public void Reload()
    {
        MainThread.BeginInvokeOnMainThread(() => _webView?.Reload());
    }

    public void Show()
    {
        if (_webView != null)
        {
            MainThread.BeginInvokeOnMainThread(() => _webView.IsVisible = true);
        }
    }

    public void Hide()
    {
        if (_webView != null)
        {
            MainThread.BeginInvokeOnMainThread(() => _webView.IsVisible = false);
        }
    }

    public void OnNavigated(string url)
    {
        Navigated?.Invoke(this, url);
    }
}
