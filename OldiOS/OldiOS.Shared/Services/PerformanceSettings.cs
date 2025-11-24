namespace OldiOS.Shared.Services;

/// <summary>
/// Service to manage performance settings and detect device capabilities
/// </summary>
public class PerformanceSettings
{
    private bool _reducedMotion = false;
    private bool _autoDetected = false;
    
    public event Action? OnSettingsChanged;
    
    /// <summary>
    /// Whether to use reduced motion for better performance on older devices
    /// </summary>
    public bool ReducedMotion 
    { 
        get => _reducedMotion;
        set
        {
            if (_reducedMotion != value)
            {
                _reducedMotion = value;
                OnSettingsChanged?.Invoke();
            }
        }
    }
    
    /// <summary>
    /// Animation duration multiplier. 1.0 = normal, 0.5 = faster, 2.0 = slower
    /// Reduced motion uses shorter durations for better performance
    /// </summary>
    public double AnimationDurationMultiplier => _reducedMotion ? 0.7 : 1.0;
    
    /// <summary>
    /// Whether to use simpler transitions (no blur, shadows, etc.)
    /// </summary>
    public bool SimplifiedTransitions => _reducedMotion;
    
    /// <summary>
    /// Detect device capabilities and automatically enable reduced motion if needed
    /// </summary>
    public void AutoDetectPerformanceSettings()
    {
        if (_autoDetected)
        {
            return; // Already detected
        }
        
        // For now, we'll leave auto-detection off by default
        // Users can manually enable reduced motion if needed
        _autoDetected = true;
    }
    
    /// <summary>
    /// Get animation duration in milliseconds, adjusted for performance settings
    /// </summary>
    public int GetAdjustedDuration(int baseMilliseconds)
    {
        return (int)(baseMilliseconds * AnimationDurationMultiplier);
    }
    
    /// <summary>
    /// Get CSS animation duration string
    /// </summary>
    public string GetCssDuration(int baseMilliseconds)
    {
        var adjusted = GetAdjustedDuration(baseMilliseconds);
        return $"{adjusted}ms";
    }
}
