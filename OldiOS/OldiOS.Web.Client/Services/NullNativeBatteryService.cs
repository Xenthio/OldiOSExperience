using OldiOS.Shared.Services;

namespace OldiOS.Web.Client.Services
{
    /// <summary>
    /// Null implementation of native battery service for web platforms.
    /// Web platforms use JavaScript Battery API instead.
    /// </summary>
    public class NullNativeBatteryService : INativeBatteryService
    {
#pragma warning disable CS0067 // Event is never used - this is a null implementation
        public event EventHandler? BatteryChanged;
#pragma warning restore CS0067

        public bool IsCharging => false;

        public double ChargeLevel => 1.0;

        public bool IsAvailable => false;
    }
}
