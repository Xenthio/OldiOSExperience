using Foundation;
using UIKit;
using OldiOS.Shared.Services;

namespace OldiOS.Services.Platforms.iOS
{
    /// <summary>
    /// iOS MAUI implementation of IHapticService using UIKit Feedback Generators.
    /// Falls back to UIFeedbackGenerators and is suitable for iOS 10+.
    /// For advanced custom patterns, consider adding a CoreHaptics implementation.
    /// </summary>
    public class MauiHapticService_iOS : IHapticService
    {
        public bool IsAvailable => UIDevice.CurrentDevice.CheckSystemVersion(10, 0);

        public void ImpactLight()
        {
            if (!IsAvailable) return;
            var g = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Light);
            g.Prepare();
            g.ImpactOccurred();
        }

        public void ImpactMedium()
        {
            if (!IsAvailable) return;
            var g = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Medium);
            g.Prepare();
            g.ImpactOccurred();
        }

        public void ImpactHeavy()
        {
            if (!IsAvailable) return;
            var g = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Heavy);
            g.Prepare();
            g.ImpactOccurred();
        }

        public void SelectionChanged()
        {
            if (!IsAvailable) return;
            var g = new UISelectionFeedbackGenerator();
            g.Prepare();
            g.SelectionChanged();
        }

        public void Success()
        {
            if (!IsAvailable) return;
            var g = new UINotificationFeedbackGenerator();
            g.Prepare();
            g.NotificationOccurred(UINotificationFeedbackType.Success);
        }

        public void Warning()
        {
            if (!IsAvailable) return;
            var g = new UINotificationFeedbackGenerator();
            g.Prepare();
            g.NotificationOccurred(UINotificationFeedbackType.Warning);
        }

        public void Error()
        {
            if (!IsAvailable) return;
            var g = new UINotificationFeedbackGenerator();
            g.Prepare();
            g.NotificationOccurred(UINotificationFeedbackType.Error);
        }

        // TODO: If you want CoreHaptics (CHHapticEngine) support, we can implement a CHHapticEngine wrapper
        // that uses CHHapticPattern/CHHapticEvent for complex patterns and fall back to the generators above.
    }
}
