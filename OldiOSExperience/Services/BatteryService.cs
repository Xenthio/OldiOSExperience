namespace OldiOSExperience.Services
{
    /// <summary>
    /// Service to manage battery state for debug and simulation purposes.
    /// Allows overriding the browser's Battery API with manual settings.
    /// </summary>
    public class BatteryService
    {
        private bool _isCharging = false;
        private int _batteryPercentage = 100;
        private bool _useManualOverride = false;

        public event Action? OnBatteryStateChanged;

        /// <summary>
        /// Gets or sets whether the device is charging.
        /// </summary>
        public bool IsCharging
        {
            get => _isCharging;
            set
            {
                if (_isCharging != value)
                {
                    _isCharging = value;
                    NotifyStateChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the battery percentage (0-100).
        /// </summary>
        public int BatteryPercentage
        {
            get => _batteryPercentage;
            set
            {
                var clampedValue = Math.Max(0, Math.Min(100, value));
                if (_batteryPercentage != clampedValue)
                {
                    _batteryPercentage = clampedValue;
                    NotifyStateChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to use manual battery override instead of browser API.
        /// </summary>
        public bool UseManualOverride
        {
            get => _useManualOverride;
            set
            {
                if (_useManualOverride != value)
                {
                    _useManualOverride = value;
                    NotifyStateChanged();
                }
            }
        }

        /// <summary>
        /// Gets the battery state enum based on current settings.
        /// </summary>
        public OldiOSExperience.System.BatteryState GetBatteryState()
        {
            if (IsCharging)
            {
                return OldiOSExperience.System.BatteryState.Charging;
            }
            else if (BatteryPercentage == 100)
            {
                return OldiOSExperience.System.BatteryState.Full;
            }
            else
            {
                return OldiOSExperience.System.BatteryState.Draining;
            }
        }

        private void NotifyStateChanged()
        {
            OnBatteryStateChanged?.Invoke();
        }
    }
}
