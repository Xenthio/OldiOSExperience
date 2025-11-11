namespace OldiOS.Shared.Services
{
    /// <summary>
    /// Device preset configurations
    /// </summary>
    public enum DevicePreset
    {
        iPhoneOriginal,  // iPhone 2G, 3G, 3GS - 320×480 @1x
        iPhone4,         // iPhone 4, 4S - 640×960 @2x
        iPhone5,         // iPhone 5, 5C, 5S - 640×1136 @2x
        iPhone6,         // iPhone 6, 6S, 7, 8 - 750×1334 @2x
        iPhone6Plus,     // iPhone 6+, 6S+, 7+, 8+ - 1080×1920 @3x (downsampled from 1242×2208)
        iPadOriginal,    // iPad 1, 2, iPad mini 1 - 768×1024 @1x
        iPad,            // iPad 3+, iPad mini 2+ - 1536×2048 @2x
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
        private double _contentScaleFactor = 2.0; // @1x = 1.0, @2x = 2.0, @3x = 3.0

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
        /// The content scale factor (@1x = 1.0, @2x = 2.0, @3x = 3.0)
        /// This represents how many pixels make up one point.
        /// For example, @2x means a 10pt icon is 20px.
        /// </summary>
        public double ContentScaleFactor
        {
            get => _contentScaleFactor;
            private set => _contentScaleFactor = value;
        }

        /// <summary>
        /// Width in logical points (px / scale factor)
        /// </summary>
        public double WidthInPoints => RESOLUTION_X / ContentScaleFactor;

        /// <summary>
        /// Height in logical points (px / scale factor)
        /// </summary>
        public double HeightInPoints => RESOLUTION_Y / ContentScaleFactor;

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
                    case DevicePreset.iPhoneOriginal:
                    case DevicePreset.iPhone4:
                    case DevicePreset.iPhone5:
                    case DevicePreset.iPhone6:
                    case DevicePreset.iPhone6Plus:
                        return DeviceType.iPhone;
                    case DevicePreset.iPadOriginal:
                    case DevicePreset.iPad:
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
                case DevicePreset.iPhoneOriginal:
                    RESOLUTION_X = 320.0;
                    RESOLUTION_Y = 480.0;
                    ContentScaleFactor = 1.0;
                    break;
                case DevicePreset.iPhone4:
                    RESOLUTION_X = 640.0;
                    RESOLUTION_Y = 960.0;
                    ContentScaleFactor = 2.0;
                    break;
                case DevicePreset.iPhone5:
                    RESOLUTION_X = 640.0;
                    RESOLUTION_Y = 1136.0;
                    ContentScaleFactor = 2.0;
                    break;
                case DevicePreset.iPhone6:
                    RESOLUTION_X = 750.0;
                    RESOLUTION_Y = 1334.0;
                    ContentScaleFactor = 2.0;
                    break;
                case DevicePreset.iPhone6Plus:
                    RESOLUTION_X = 1080.0;
                    RESOLUTION_Y = 1920.0;
                    ContentScaleFactor = 3.0;
                    break;
                case DevicePreset.iPadOriginal:
                    RESOLUTION_X = 768.0;
                    RESOLUTION_Y = 1024.0;
                    ContentScaleFactor = 1.0;
                    break;
                case DevicePreset.iPad:
                    RESOLUTION_X = 1536.0;
                    RESOLUTION_Y = 2048.0;
                    ContentScaleFactor = 2.0;
                    break;
                case DevicePreset.Custom:
                    // Custom - don't change resolution or scale
                    break;
            }
        }

        /// <summary>
        /// Set custom resolution
        /// </summary>
        public void SetCustomResolution(double width, double height, double contentScale = 2.0)
        {
            CurrentPreset = DevicePreset.Custom;
            RESOLUTION_X = width;
            RESOLUTION_Y = height;
            ContentScaleFactor = contentScale;
        }

        /// <summary>
        /// Get the image path suffix for the current scale (@1x, @2x, @3x)
        /// </summary>
        public string GetImageScaleSuffix()
        {
            return ContentScaleFactor switch
            {
                1.0 => "",      // @1x has no suffix (e.g., "icon.png")
                2.0 => "@2x",   // @2x (e.g., "icon@2x.png")
                3.0 => "@3x",   // @3x (e.g., "icon@3x.png")
                _ => "@2x"      // Default to @2x for any other scale
            };
        }

        /// <summary>
        /// Get the best matching image path for the current scale factor.
        /// Tries exact match first, then falls back to closest available scale.
        /// </summary>
        /// <param name="basePath">Base path without scale suffix (e.g., "images/icon.png")</param>
        /// <returns>Best matching image path with scale suffix</returns>
        public string GetScaledImagePath(string basePath)
        {
            // If the path already contains a scale suffix, return as-is
            if (basePath.Contains("@2x") || basePath.Contains("@3x"))
            {
                return basePath;
            }

            // Split the path into name and extension
            var lastDot = basePath.LastIndexOf('.');
            if (lastDot == -1)
            {
                return basePath; // No extension, return as-is
            }

            var pathWithoutExt = basePath.Substring(0, lastDot);
            var extension = basePath.Substring(lastDot);

            // Get the suffix for current scale
            var suffix = GetImageScaleSuffix();
            
            // Return path with appropriate suffix
            return $"{pathWithoutExt}{suffix}{extension}";
        }
    }
}