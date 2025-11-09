using OldiOS.Shared.Models;

namespace OldiOS.Shared.Services
{
    /// <summary>
    /// Service for coordinating complex multi-element animations
    /// Inspired by iOS's Core Animation
    /// </summary>
    public class AnimationService
    {
        /// <summary>
        /// Coordinates app open animation sequence:
        /// 1. Icons animate radially outward from center
        /// 2. Dock slides down
        /// 3. Everything except the opening app fades to black
        /// 4. App zooms in from center
        /// </summary>
        public AnimationSequence CreateAppOpenSequence()
        {
            return new AnimationSequence
            {
                Name = "AppOpen",
                TotalDurationMs = 400,
                Steps = new()
                {
                    // Step 1: Icons scatter and fade (simultaneous)
                    new AnimationStep
                    {
                        StartTimeMs = 0,
                        DurationMs = 300,
                        Targets = new() { "springboard-icons", "dock" },
                        Properties = new() 
                        { 
                            { "fade", "true" },
                            { "radial-scatter", "true" }
                        }
                    },
                    // Step 2: App zooms in from center (slightly delayed)
                    new AnimationStep
                    {
                        StartTimeMs = 100,
                        DurationMs = 300,
                        Targets = new() { "app-container" },
                        Properties = new()
                        {
                            { "scale-from", "0.1" },
                            { "scale-to", "1.0" },
                            { "opacity-from", "0" },
                            { "opacity-to", "1" }
                        }
                    }
                }
            };
        }
        
        /// <summary>
        /// Coordinates app close animation sequence (reverse of open)
        /// </summary>
        public AnimationSequence CreateAppCloseSequence()
        {
            return new AnimationSequence
            {
                Name = "AppClose",
                TotalDurationMs = 400,
                Steps = new()
                {
                    // Step 1: App zooms out to center
                    new AnimationStep
                    {
                        StartTimeMs = 0,
                        DurationMs = 300,
                        Targets = new() { "app-container" },
                        Properties = new()
                        {
                            { "scale-from", "1.0" },
                            { "scale-to", "0.1" },
                            { "opacity-from", "1" },
                            { "opacity-to", "0" }
                        }
                    },
                    // Step 2: Icons gather and fade in (simultaneous, slightly delayed)
                    new AnimationStep
                    {
                        StartTimeMs = 100,
                        DurationMs = 300,
                        Targets = new() { "springboard-icons", "dock" },
                        Properties = new()
                        {
                            { "fade-in", "true" },
                            { "radial-gather", "true" }
                        }
                    }
                }
            };
        }
        
        /// <summary>
        /// Creates unlock animation sequence
        /// </summary>
        public AnimationSequence CreateUnlockSequence()
        {
            return new AnimationSequence
            {
                Name = "Unlock",
                TotalDurationMs = 400,
                Steps = new()
                {
                    new AnimationStep
                    {
                        StartTimeMs = 0,
                        DurationMs = 400,
                        Targets = new() { "lock-screen" },
                        Properties = new()
                        {
                            { "slide-direction", "right" },
                            { "opacity-from", "1" },
                            { "opacity-to", "0" }
                        }
                    }
                }
            };
        }
        
        /// <summary>
        /// Get CSS class for animation
        /// </summary>
        public string GetAnimationClass(string animationName, bool isAnimating)
        {
            return isAnimating ? $"animating-{animationName.ToLower()}" : "";
        }
    }
    
    /// <summary>
    /// Represents a sequence of coordinated animations
    /// </summary>
    public class AnimationSequence
    {
        public string Name { get; set; } = "";
        public int TotalDurationMs { get; set; }
        public List<AnimationStep> Steps { get; set; } = new();
    }
    
    /// <summary>
    /// Represents a single step in an animation sequence
    /// </summary>
    public class AnimationStep
    {
        public int StartTimeMs { get; set; }
        public int DurationMs { get; set; }
        public List<string> Targets { get; set; } = new();
        public Dictionary<string, string> Properties { get; set; } = new();
    }
}
