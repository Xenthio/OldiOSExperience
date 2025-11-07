namespace OldiOSExperience.Services
{
    public class AnimationService
    {
        // Animation timing constants (in milliseconds)
        public const int ANIMATION_DURATION_MS = 400;
        public const int ANIMATION_START_DELAY_MS = 50;
        
        // Screen dimensions for iOS 5/6 (iPhone 4 resolution)
        public const double SCREEN_CENTER_X = 320;
        public const double SCREEN_CENTER_Y = 480;
        
        public event Action? OnAnimationStateChanged;
        
        public bool IsAnimating { get; private set; }
        public AnimationState CurrentState { get; private set; } = AnimationState.Idle;
        
        // Store the center point of the clicked app icon
        public double CenterX { get; private set; }
        public double CenterY { get; private set; }
        
        public void StartOpeningAnimation(double centerX, double centerY)
        {
            IsAnimating = true;
            CurrentState = AnimationState.Opening;
            CenterX = centerX;
            CenterY = centerY;
            OnAnimationStateChanged?.Invoke();
        }
        
        public void StartClosingAnimation()
        {
            IsAnimating = true;
            CurrentState = AnimationState.Closing;
            OnAnimationStateChanged?.Invoke();
        }
        
        public void CompleteAnimation()
        {
            IsAnimating = false;
            CurrentState = AnimationState.Idle;
            OnAnimationStateChanged?.Invoke();
        }
    }
    
    public enum AnimationState
    {
        Idle,
        Opening,
        Closing
    }
}
