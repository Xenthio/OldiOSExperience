using Microsoft.Maui.Devices;
using OldiOS.Shared.Services;

namespace OldiOS.Services
{
    /// <summary>
    /// MAUI implementation of haptic service using Microsoft.Maui.Devices.HapticFeedback API.
    /// </summary>
    public class MauiHapticService : IHapticService
    {
        public bool IsAvailable => HapticFeedback.Default.IsSupported;

        public void ImpactLight()
        {
            if (IsAvailable)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
        }

        public void ImpactMedium()
        {
            if (IsAvailable)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
        }

        public void ImpactHeavy()
        {
            if (IsAvailable)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            }
        }

        public void SelectionChanged()
        {
            if (IsAvailable)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
        }

        public void Success()
        {
            if (IsAvailable)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
        }

        public void Warning()
        {
            if (IsAvailable)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
        }

        public void Error()
        {
            if (IsAvailable)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            }
        }
    }
}
