using OldiOS.Shared.Services;

namespace OldiOS.Web.Client.Services
{
    /// <summary>
    /// Null implementation of haptic service for web platforms.
    /// Web platforms may use Vibration API via JavaScript instead.
    /// </summary>
    public class NullHapticService : IHapticService
    {
        public bool IsAvailable => false;

        public void ImpactLight() { }
        public void ImpactMedium() { }
        public void ImpactHeavy() { }
        public void SelectionChanged() { }
        public void Success() { }
        public void Warning() { }
        public void Error() { }
    }
}
