using Microsoft.Maui.Devices;
using OldiOS.Shared.Services;

namespace OldiOS.Services
{
    /// <summary>
    /// MAUI implementation of native battery service using Microsoft.Maui.Devices.Battery API.
    /// </summary>
    public class MauiNativeBatteryService : INativeBatteryService
    {
        public event EventHandler? BatteryChanged;

        public MauiNativeBatteryService()
        {
            // Subscribe to MAUI battery events
            Battery.BatteryInfoChanged += OnBatteryInfoChanged;
        }

        public bool IsCharging => Battery.ChargeLevel >= 1.0 || Battery.State == BatteryState.Charging || Battery.State == BatteryState.Full;

        public double ChargeLevel => Battery.ChargeLevel;

        public bool IsAvailable => true;

        private void OnBatteryInfoChanged(object? sender, BatteryInfoChangedEventArgs e)
        {
            BatteryChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
