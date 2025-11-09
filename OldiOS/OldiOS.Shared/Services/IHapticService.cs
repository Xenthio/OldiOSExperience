namespace OldiOS.Shared.Services
{
    /// <summary>
    /// Interface for platform-specific haptic feedback services.
    /// Implemented by native platforms (MAUI) to provide haptic feedback.
    /// </summary>
    public interface IHapticService
    {
        /// <summary>
        /// Gets whether haptic feedback is available on this platform.
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Performs a light impact haptic feedback (e.g., for button press).
        /// </summary>
        void ImpactLight();

        /// <summary>
        /// Performs a medium impact haptic feedback.
        /// </summary>
        void ImpactMedium();

        /// <summary>
        /// Performs a heavy impact haptic feedback (e.g., for 3D Touch activation).
        /// </summary>
        void ImpactHeavy();

        /// <summary>
        /// Performs a selection changed haptic feedback.
        /// </summary>
        void SelectionChanged();

        /// <summary>
        /// Performs a success haptic feedback.
        /// </summary>
        void Success();

        /// <summary>
        /// Performs a warning haptic feedback.
        /// </summary>
        void Warning();

        /// <summary>
        /// Performs an error haptic feedback.
        /// </summary>
        void Error();
    }
}
