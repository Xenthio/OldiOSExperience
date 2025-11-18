namespace OldiOS.Shared.Services;

/// <summary>
/// Interface for Safari browser implementation across different platforms
/// </summary>
public interface ISafariService
{
    /// <summary>
    /// Load a URL in the browser
    /// </summary>
    Task<bool> LoadUrl(string url);
    
    /// <summary>
    /// Navigate back in history
    /// </summary>
    Task<bool> NavigateBack();
    
    /// <summary>
    /// Navigate forward in history
    /// </summary>
    Task<bool> NavigateForward();
    
    /// <summary>
    /// Reload the current page
    /// </summary>
    Task Reload();
    
    /// <summary>
    /// Get the current URL
    /// </summary>
    string GetCurrentUrl();
    
    /// <summary>
    /// Check if can go back
    /// </summary>
    bool CanGoBack { get; }
    
    /// <summary>
    /// Check if can go forward
    /// </summary>
    bool CanGoForward { get; }
    
    /// <summary>
    /// Event raised when navigation completes
    /// </summary>
    event EventHandler<string>? NavigationCompleted;
    
    /// <summary>
    /// Event raised when navigation fails
    /// </summary>
    event EventHandler<string>? NavigationFailed;
}
