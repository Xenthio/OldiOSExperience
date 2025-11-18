namespace OldiOS.Services;

/// <summary>
/// Web platform implementation (no-op since web uses iframe)
/// </summary>
public class SafariWebViewService : ISafariWebViewService
{
    public event EventHandler<string>? Navigated;

    public void NavigateToUrl(string url)
    {
        // No-op on web platform
    }

    public void GoBack()
    {
        // No-op on web platform
    }

    public void GoForward()
    {
        // No-op on web platform
    }

    public void Reload()
    {
        // No-op on web platform
    }

    public void Show()
    {
        // No-op on web platform
    }

    public void Hide()
    {
        // No-op on web platform
    }
}
