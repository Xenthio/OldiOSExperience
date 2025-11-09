namespace OldiOS.Shared.Services
{
    /// <summary>
    /// Device preset configurations
    /// </summary>
    public enum DevicePreset
    {
        iPhone4,
        iPhone5,
        iPhone6,
        iPhone6Plus,
        iPad,
        iPadMini,
        Custom
    }

    /// <summary>
    /// Device type for determining grid layout
    /// </summary>
    public enum DeviceType
    {
        iPhone,
        iPad
    }

    /// <summary>
    /// Service managing display settings for the simulated iOS device
    /// </summary>
    public class DisplaySettings
    {
        private double _resolutionX = 640.0;
        private double _resolutionY = 960.0;
        private double _scaleFactor = 1.0;
        private DevicePreset _currentPreset = DevicePreset.iPhone4;

        /// <summary>
        /// Event fired when resolution changes
        /// </summary>
        public event Action? OnResolutionChanged;

        /// <summary>
        /// The native horizontal resolution of the simulated screen.
        /// </summary>
        public double RESOLUTION_X
        {
            get => _resolutionX;
            set
            {
                if (_resolutionX != value)
                {
                    _resolutionX = value;
                    OnResolutionChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// Alias for RESOLUTION_X - the screen width
        /// </summary>
        public double Width => RESOLUTION_X;

        /// <summary>
        /// The native vertical resolution of the simulated screen.
        /// </summary>
        public double RESOLUTION_Y
        {
            get => _resolutionY;
            set
            {
                if (_resolutionY != value)
                {
                    _resolutionY = value;
                    OnResolutionChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// The calculated scale factor to fit the screen in the browser viewport.
        /// </summary>
        public double SCALEFACTOR
        {
            get => _scaleFactor;
            set => _scaleFactor = value;
        }

        /// <summary>
        /// Current device preset
        /// </summary>
        public DevicePreset CurrentPreset
        {
            get => _currentPreset;
            set => _currentPreset = value;
        }

        /// <summary>
        /// Get the device type based on resolution
        /// </summary>
        public DeviceType GetDeviceType()
        {
            // Use the preset for a more reliable check
            if (CurrentPreset != DevicePreset.Custom)
            {
                switch (CurrentPreset)
                {
                    case DevicePreset.iPhone4:
                    case DevicePreset.iPhone5:
                    case DevicePreset.iPhone6:
                    case DevicePreset.iPhone6Plus:
                        return DeviceType.iPhone;
                    case DevicePreset.iPad:
                    case DevicePreset.iPadMini:
                        return DeviceType.iPad;
                }
            }

            // Fallback for custom resolutions: check aspect ratio.
            // iPads are 4:3, iPhones are taller.
            double aspectRatio = Math.Max(RESOLUTION_X, RESOLUTION_Y) / Math.Min(RESOLUTION_X, RESOLUTION_Y);
            if (aspectRatio < 1.5) // 4:3 is ~1.33. 3:2 (iPhone 4) is 1.5. 16:9 is ~1.77
            {
                return DeviceType.iPad;
            }
            
            return DeviceType.iPhone;
        }

        /// <summary>
        /// Get the number of icon rows for the springboard based on device type and resolution
        /// </summary>
        public int GetIconRows()
        {
            var deviceType = GetDeviceType();
            bool isLandscape = RESOLUTION_X > RESOLUTION_Y;

            if (deviceType == DeviceType.iPad)
            {
                // iPad: 5 rows in portrait, 4 in landscape
                return isLandscape ? 4 : 5;
            }
            else
            {
                // iPhone: 4 rows for iPhone 4 (960px), 5 rows for iPhone 5+ (1136px+)
                // Use the portrait height for this check
                double portraitHeight = Math.Max(RESOLUTION_X, RESOLUTION_Y);
                return portraitHeight >= 1136 ? 5 : 4;
            }
        }

        /// <summary>
        /// Get the number of icon columns for the springboard based on device type
        /// </summary>
        public int GetIconColumns()
        {
            var deviceType = GetDeviceType();
            bool isLandscape = RESOLUTION_X > RESOLUTION_Y;

            if (deviceType == DeviceType.iPad && isLandscape)
            {
                // iPads in landscape have 5 columns
                return 5;
            }
            
            // iPhones and iPads in portrait have 4 columns
            return 4;
        }

        /// <summary>
        /// Set resolution to a preset device
        /// </summary>
        public void SetPreset(DevicePreset preset)
        {
            CurrentPreset = preset;
            switch (preset)
            {
                case DevicePreset.iPhone4:
                    RESOLUTION_X = 640.0;
                    RESOLUTION_Y = 960.0;
                    break;
                case DevicePreset.iPhone5:
                    RESOLUTION_X = 640.0;
                    RESOLUTION_Y = 1136.0;
                    break;
                case DevicePreset.iPhone6:
                    RESOLUTION_X = 750.0;
                    RESOLUTION_Y = 1334.0;
                    break;
                case DevicePreset.iPhone6Plus:
                    RESOLUTION_X = 1080.0;
                    RESOLUTION_Y = 1920.0;
                    break;
                case DevicePreset.iPad:
                    RESOLUTION_X = 1536.0;
                    RESOLUTION_Y = 2048.0;
                    break;
                case DevicePreset.iPadMini:
                    RESOLUTION_X = 768;
                    RESOLUTION_Y = 1024;
                    break;
                case DevicePreset.Custom:
                    // Custom - don't change resolution
                    break;
            }
        }

        /// <summary>
        /// Set custom resolution
        /// </summary>
        public void SetCustomResolution(double width, double height)
        {
            CurrentPreset = DevicePreset.Custom;
            RESOLUTION_X = width;
            RESOLUTION_Y = height;
        }
    }
}