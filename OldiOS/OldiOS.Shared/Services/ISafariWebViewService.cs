namespace OldiOS.Services;

/// <summary>
/// Service for controlling the native Safari WebView from Blazor
/// </summary>
public interface ISafariWebViewService
{
    void NavigateToUrl(string url);
    void GoBack();
    void GoForward();
    void Reload();
    void Show();
    void Hide();
    
    event EventHandler<string>? Navigated;
}
