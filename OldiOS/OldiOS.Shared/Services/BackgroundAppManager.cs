using OldiOS.Shared.Models;

namespace OldiOS.Shared.Services
{
    /// <summary>
    /// Manages app multitasking and background state
    /// Inspired by iOS's UIApplication and multitasking system
    /// </summary>
    public class BackgroundAppManager
    {
        private readonly Dictionary<int, AppState> _appStates = new();
        private readonly List<AppState> _recentApps = new(); // For app switcher
        private const int MaxRecentApps = 10;
        private AppState? _cachedForegroundApp = null;
        
        public event Action? OnAppStatesChanged;
        
        /// <summary>Get current foreground app</summary>
        public AppState? ForegroundApp => _cachedForegroundApp;
        
        /// <summary>Get all background apps</summary>
        public IEnumerable<AppState> BackgroundApps => _appStates.Values.Where(s => s.ExecutionState == AppExecutionState.Background);
        
        /// <summary>Get recent apps for app switcher</summary>
        public IReadOnlyList<AppState> RecentApps => _recentApps.AsReadOnly();
        
        /// <summary>
        /// Launch or bring app to foreground
        /// </summary>
        public void LaunchApp(AppInfo app)
        {
            // Move current foreground app to background
            if (_cachedForegroundApp != null)
            {
                MoveToBackground(_cachedForegroundApp);
            }
            
            // Get or create app state
            if (!_appStates.TryGetValue(app.Id, out var appState))
            {
                appState = new AppState { App = app };
                _appStates[app.Id] = appState;
            }
            
            // Move to foreground
            appState.ExecutionState = AppExecutionState.Foreground;
            _cachedForegroundApp = appState;
            
            // Add to recent apps (remove if already in list, then add to front)
            _recentApps.RemoveAll(s => s.App.Id == app.Id);
            _recentApps.Insert(0, appState);
            
            // Keep only most recent apps
            if (_recentApps.Count > MaxRecentApps)
            {
                _recentApps.RemoveRange(MaxRecentApps, _recentApps.Count - MaxRecentApps);
            }
            
            OnAppStatesChanged?.Invoke();
        }
        
        /// <summary>
        /// Close app completely (remove from background)
        /// </summary>
        public void CloseApp(int appId)
        {
            if (_appStates.TryGetValue(appId, out var appState))
            {
                appState.ExecutionState = AppExecutionState.NotRunning;
                _appStates.Remove(appId);
                _recentApps.RemoveAll(s => s.App.Id == appId);
                OnAppStatesChanged?.Invoke();
            }
        }
        
        /// <summary>
        /// Return to springboard (home screen)
        /// </summary>
        public void ReturnToSpringboard()
        {
            if (_cachedForegroundApp != null)
            {
                MoveToBackground(_cachedForegroundApp);
                _cachedForegroundApp = null;
                OnAppStatesChanged?.Invoke();
            }
        }
        
        /// <summary>
        /// Suspend all background apps (memory management simulation)
        /// </summary>
        public void SuspendBackgroundApps()
        {
            foreach (var appState in BackgroundApps)
            {
                appState.ExecutionState = AppExecutionState.Suspended;
            }
            OnAppStatesChanged?.Invoke();
        }
        
        /// <summary>
        /// Get app state by ID
        /// </summary>
        public AppState? GetAppState(int appId)
        {
            return _appStates.TryGetValue(appId, out var state) ? state : null;
        }
        
        /// <summary>
        /// Check if app is running (foreground or background)
        /// </summary>
        public bool IsAppRunning(int appId)
        {
            return _appStates.TryGetValue(appId, out var state) && 
                   state.ExecutionState != AppExecutionState.NotRunning;
        }
        
        private void MoveToBackground(AppState appState)
        {
            appState.ExecutionState = AppExecutionState.Background;
            appState.BackgroundedAt = DateTime.Now;
        }
    }
}
