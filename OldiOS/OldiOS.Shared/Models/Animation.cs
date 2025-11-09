namespace OldiOS.Shared.Models
{
    /// <summary>
    /// Defines an animation that can be applied to elements
    /// </summary>
    public class Animation
    {
        public string Name { get; set; } = "";
        public int DurationMs { get; set; } = 300;
        public string Easing { get; set; } = "ease-out";
        public Dictionary<string, AnimationKeyframe> Keyframes { get; set; } = new();
    }
    
    /// <summary>
    /// Represents a keyframe in an animation
    /// </summary>
    public class AnimationKeyframe
    {
        public Dictionary<string, string> Properties { get; set; } = new();
    }
    
    /// <summary>
    /// Predefined iOS-style animations
    /// </summary>
    public static class iOSAnimations
    {
        /// <summary>App opening zoom animation (from center)</summary>
        public static Animation AppOpen => new()
        {
            Name = "appOpen",
            DurationMs = 300,
            Easing = "ease-out",
            Keyframes = new()
            {
                ["from"] = new() { Properties = new() { ["transform"] = "scale(0.1)", ["opacity"] = "0" } },
                ["to"] = new() { Properties = new() { ["transform"] = "scale(1)", ["opacity"] = "1" } }
            }
        };
        
        /// <summary>App closing zoom animation (to center)</summary>
        public static Animation AppClose => new()
        {
            Name = "appClose",
            DurationMs = 300,
            Easing = "ease-in",
            Keyframes = new()
            {
                ["from"] = new() { Properties = new() { ["transform"] = "scale(1)", ["opacity"] = "1" } },
                ["to"] = new() { Properties = new() { ["transform"] = "scale(0.1)", ["opacity"] = "0" } }
            }
        };
        
        /// <summary>Fade in animation</summary>
        public static Animation FadeIn => new()
        {
            Name = "fadeIn",
            DurationMs = 200,
            Easing = "ease-out",
            Keyframes = new()
            {
                ["from"] = new() { Properties = new() { ["opacity"] = "0" } },
                ["to"] = new() { Properties = new() { ["opacity"] = "1" } }
            }
        };
        
        /// <summary>Fade out animation</summary>
        public static Animation FadeOut => new()
        {
            Name = "fadeOut",
            DurationMs = 200,
            Easing = "ease-in",
            Keyframes = new()
            {
                ["from"] = new() { Properties = new() { ["opacity"] = "1" } },
                ["to"] = new() { Properties = new() { ["opacity"] = "0" } }
            }
        };
        
        /// <summary>Slide up animation (dock/elements coming up)</summary>
        public static Animation SlideUp => new()
        {
            Name = "slideUp",
            DurationMs = 300,
            Easing = "ease-out",
            Keyframes = new()
            {
                ["from"] = new() { Properties = new() { ["transform"] = "translateY(100%)", ["opacity"] = "0" } },
                ["to"] = new() { Properties = new() { ["transform"] = "translateY(0)", ["opacity"] = "1" } }
            }
        };
        
        /// <summary>Slide down animation (dock/elements going down)</summary>
        public static Animation SlideDown => new()
        {
            Name = "slideDown",
            DurationMs = 300,
            Easing = "ease-in",
            Keyframes = new()
            {
                ["from"] = new() { Properties = new() { ["transform"] = "translateY(0)", ["opacity"] = "1" } },
                ["to"] = new() { Properties = new() { ["transform"] = "translateY(100%)", ["opacity"] = "0" } }
            }
        };
        
        /// <summary>Unlock swipe animation</summary>
        public static Animation UnlockSwipe => new()
        {
            Name = "unlockSwipe",
            DurationMs = 400,
            Easing = "ease-out",
            Keyframes = new()
            {
                ["from"] = new() { Properties = new() { ["transform"] = "translateX(0)" } },
                ["to"] = new() { Properties = new() { ["transform"] = "translateX(100%)", ["opacity"] = "0" } }
            }
        };
    }
}
