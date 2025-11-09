namespace OldiOS.Shared.Services
{
    /// <summary>
    /// Service to manage battery state for debug and simulation purposes.
    /// Supports native battery API (MAUI), browser Battery API (web), and manual override.
    /// </summary>
    public class BatteryService
    {
        private readonly INativeBatteryService? _nativeBatteryService;
        private bool _isCharging = false;
        private int _batteryPercentage = 100;
        private bool _useManualOverride = false;

        public event Action? OnBatteryStateChanged;

        /// <summary>
        /// Initializes a new instance of BatteryService.
        /// </summary>
        /// <param name="nativeBatteryService">Optional native battery service for MAUI platforms.</param>
        public BatteryService(INativeBatteryService? nativeBatteryService = null)
        {
            _nativeBatteryService = nativeBatteryService;
            
            // Subscribe to native battery changes if available
            if (_nativeBatteryService?.IsAvailable == true)
            {
                _nativeBatteryService.BatteryChanged += OnNativeBatteryChanged;
                // Initialize with current native values
                UpdateFromNative();
            }
        }

        /// <summary>
        /// Gets whether native battery information is available (MAUI platforms).
        /// </summary>
        public bool HasNativeSupport => _nativeBatteryService?.IsAvailable == true;

        private void OnNativeBatteryChanged(object? sender, EventArgs e)
        {
            if (!UseManualOverride)
            {
                UpdateFromNative();
            }
        }

        private void UpdateFromNative()
        {
            if (_nativeBatteryService?.IsAvailable == true)
            {
                _isCharging = _nativeBatteryService.IsCharging;
                _batteryPercentage = (int)Math.Round(_nativeBatteryService.ChargeLevel * 100);
                NotifyStateChanged();
            }
        }

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
        public OldiOS.Shared.System.BatteryState GetBatteryState()
        {
            if (IsCharging)
            {
                if (BatteryPercentage == 100)
                {
                    return OldiOS.Shared.System.BatteryState.Full;
                }
                return OldiOS.Shared.System.BatteryState.Charging;
            }
            else
            {
                return OldiOS.Shared.System.BatteryState.Draining;
            }
        }

        private void NotifyStateChanged()
        {
            OnBatteryStateChanged?.Invoke();
        }
    }
}
