using OldiOSExperience.Models;

namespace OldiOSExperience.Services
{
    /// <summary>
    /// Service managing the springboard (home screen) state and app layout
    /// Inspired by iOS's SpringBoard daemon
    /// </summary>
    public class SpringboardService
    {
        private readonly BackgroundAppManager _appManager;
        private readonly AnimationService _animationService;
        
        public SpringboardService(BackgroundAppManager appManager, AnimationService animationService)
        {
            _appManager = appManager;
            _animationService = animationService;
        }
        
        public List<List<AppInfo>> Pages { get; private set; } = new();
        public List<AppInfo> DockApps { get; private set; } = new();
        public List<AppInfo> AllApps { get; private set; } = new();
        
        public event Action? OnLayoutChanged;
        
        /// <summary>
        /// Initialize springboard with app layout
        /// </summary>
        public void InitializeLayout()
        {
            // Dock apps
            DockApps = new List<AppInfo>
            {
                new AppInfo { Id = 101, Name = "Phone", IconPath = "images/icons/phone.png", BundleId = "com.apple.mobilephone" },
                new AppInfo { Id = 102, Name = "Mail", IconPath = "images/icons/mail.png", BundleId = "com.apple.mobilemail" },
                new AppInfo { Id = 103, Name = "Safari", IconPath = "images/icons/safari.png", BundleId = "com.apple.mobilesafari" },
                new AppInfo { Id = 104, Name = "Music", IconPath = "images/icons/music.png", BundleId = "com.apple.music" }
            };

            // Page 1
            var page1 = new List<AppInfo>
            {
                new AppInfo { Id = 1, Name = "Messages", IconPath = "images/icons/messages.png", BundleId = "com.apple.mobilesms" },
                new AppInfo { Id = 2, Name = "Calendar", IconPath = "images/icons/calendar.png", BundleId = "com.apple.mobilecal" },
                new AppInfo { Id = 3, Name = "Photos", IconPath = "images/icons/photos.png", BundleId = "com.apple.mobileslideshow" },
                new AppInfo { Id = 4, Name = "Camera", IconPath = "images/icons/camera.png", BundleId = "com.apple.camera" },
                new AppInfo { Id = 5, Name = "YouTube", IconPath = "images/icons/youtube.png", BundleId = "com.google.youtube" },
                new AppInfo { Id = 6, Name = "Stocks", IconPath = "images/icons/stocks.png", BundleId = "com.apple.stocks" },
                new AppInfo { Id = 7, Name = "Maps", IconPath = "images/icons/maps.png", BundleId = "com.apple.Maps" },
                new AppInfo { Id = 8, Name = "Weather", IconPath = "images/icons/weather.png", BundleId = "com.apple.weather" }
            };

            // Page 2
            var page2 = new List<AppInfo>
            {
                new AppInfo { Id = 9, Name = "Notes", IconPath = "images/icons/notes.png", BundleId = "com.apple.mobilenotes" },
                new AppInfo { Id = 10, Name = "Reminders", IconPath = "images/icons/reminders.png", BundleId = "com.apple.reminders" },
                new AppInfo { Id = 11, Name = "Clock", IconPath = "images/icons/clock.png", BundleId = "com.apple.mobiletimer" },
                new AppInfo { Id = 12, Name = "Videos", IconPath = "images/icons/videos.png", BundleId = "com.apple.videos" },
                new AppInfo { Id = 13, Name = "App Store", IconPath = "images/icons/appstore.png", BundleId = "com.apple.AppStore" },
                new AppInfo { Id = 14, Name = "iTunes", IconPath = "images/icons/itunes.png", BundleId = "com.apple.MobileStore" },
                new AppInfo { Id = 15, Name = "Game Center", IconPath = "images/icons/gamecenter.png", BundleId = "com.apple.gamecenter" },
                new AppInfo { Id = 16, Name = "Settings", IconPath = "images/icons/settings.png", BundleId = "com.apple.Preferences", ComponentType = typeof(Apps.Settings.SettingsApp) }
            };

            Pages.Add(page1);
            Pages.Add(page2);
            
            // Create combined list of all apps for Spotlight search
            AllApps = DockApps.Concat(Pages.SelectMany(page => page)).ToList();
            
            OnLayoutChanged?.Invoke();
        }
        
        /// <summary>
        /// Launch an app from the springboard
        /// </summary>
        public void LaunchApp(AppInfo app)
        {
            _appManager.LaunchApp(app);
        }
        
        /// <summary>
        /// Handle home button press
        /// </summary>
        public void HandleHomeButton()
        {
            _appManager.ReturnToSpringboard();
        }
        
        /// <summary>
        /// Get app info by ID
        /// </summary>
        public AppInfo? GetAppById(int id)
        {
            return AllApps.FirstOrDefault(a => a.Id == id);
        }
    }
}
