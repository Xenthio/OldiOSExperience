namespace OldiOS.Shared.Models
{
    /// <summary>
    /// Represents the execution state of an app
    /// </summary>
    public enum AppExecutionState
    {
        /// <summary>App is not running</summary>
        NotRunning,
        
        /// <summary>App is in the foreground and actively running</summary>
        Foreground,
        
        /// <summary>App is running in the background</summary>
        Background,
        
        /// <summary>App is suspended (in memory but not executing)</summary>
        Suspended
    }
    
    /// <summary>
    /// Represents the state of an app in the system
    /// </summary>
    public class AppState
    {
        public required AppInfo App { get; set; }
        public AppExecutionState ExecutionState { get; set; } = AppExecutionState.NotRunning;
        
        /// <summary>Timestamp when app was last put in background</summary>
        public DateTime? BackgroundedAt { get; set; }
        
        /// <summary>App-specific state data (for future use)</summary>
        public Dictionary<string, object> StateData { get; set; } = new();
        
        /// <summary>Saved state snapshot for resumable apps (null if no state saved)</summary>
        public Dictionary<string, object>? SavedStateSnapshot { get; set; } = null;
        
        /// <summary>Reference to the app component instance (used for state save/restore)</summary>
        public object? ComponentInstance { get; set; } = null;
    }
}
