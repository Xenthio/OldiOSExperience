using OldiOS.Shared.Models;

namespace OldiOS.Shared.Services
{
    /// <summary>
    /// Service managing the springboard (home screen) state and app layout
    /// Inspired by iOS's SpringBoard daemon
    /// </summary>
    public class SpringboardService
    {
        private readonly BackgroundAppManager _appManager;
        private readonly AnimationService _animationService;
        private readonly DisplaySettings _displaySettings;
        
        public SpringboardService(BackgroundAppManager appManager, AnimationService animationService, DisplaySettings displaySettings)
        {
            _appManager = appManager;
            _animationService = animationService;
            _displaySettings = displaySettings;
            
            // Subscribe to resolution changes to update icon paths
            _displaySettings.OnResolutionChanged += HandleResolutionChanged;
        }
        
        public List<List<AppInfo>> Pages { get; private set; } = new();
        public List<AppInfo> DockApps { get; private set; } = new();
        public List<AppInfo> AllApps { get; private set; } = new();
        
        public event Action? OnLayoutChanged;
        
        private void HandleResolutionChanged()
        {
            // Reinitialize layout with new scaled icon paths
            InitializeLayout();
        }
        
        /// <summary>
        /// Get the scaled icon path for the current display settings
        /// </summary>
        private string GetScaledIconPath(string iconName)
        {
            return _displaySettings.GetScaledImagePath($"_content/OldiOS.Shared/images/icons/{iconName}.png");
        }
        
        /// <summary>
        /// Initialize springboard with app layout
        /// </summary>
        public void InitializeLayout()
        {
            // Dock apps
            DockApps = new List<AppInfo>
            {
                new AppInfo { Id = 101, Name = "Phone", IconPath = GetScaledIconPath("phone"), BundleId = "com.apple.mobilephone" },
                new AppInfo { Id = 102, Name = "Mail", IconPath = GetScaledIconPath("mail"), BundleId = "com.apple.mobilemail" },
                new AppInfo { Id = 103, Name = "Safari", IconPath = GetScaledIconPath("safari"), BundleId = "com.apple.mobilesafari" },
                new AppInfo { Id = 104, Name = "Music", IconPath = GetScaledIconPath("music"), BundleId = "com.apple.music" }
            };

            // Page 1
            var page1 = new List<AppInfo>
            {
                new AppInfo { Id = 1, Name = "Messages", IconPath = GetScaledIconPath("messages"), BundleId = "com.apple.mobilesms" },
                new AppInfo { Id = 2, Name = "Calendar", IconPath = GetScaledIconPath("calendar"), BundleId = "com.apple.mobilecal" },
                new AppInfo { Id = 3, Name = "Photos", IconPath = GetScaledIconPath("photos"), BundleId = "com.apple.mobileslideshow" },
                new AppInfo { Id = 4, Name = "Camera", IconPath = GetScaledIconPath("camera"), BundleId = "com.apple.camera" },
                new AppInfo { Id = 5, Name = "YouTube", IconPath = GetScaledIconPath("youtube"), BundleId = "com.google.youtube" },
                new AppInfo { Id = 6, Name = "Stocks", IconPath = GetScaledIconPath("stocks"), BundleId = "com.apple.stocks" },
                new AppInfo { Id = 7, Name = "Maps", IconPath = GetScaledIconPath("maps"), BundleId = "com.apple.Maps" },
                new AppInfo { Id = 8, Name = "Weather", IconPath = GetScaledIconPath("weather"), BundleId = "com.apple.weather" },
                new AppInfo { Id = 9, Name = "Notes", IconPath = GetScaledIconPath("notes"), BundleId = "com.apple.mobilenotes" },
                new AppInfo { Id = 10, Name = "Clock", IconPath = GetScaledIconPath("clock"), BundleId = "com.apple.mobiletimer" },
                new AppInfo { Id = 11, Name = "App Store", IconPath = GetScaledIconPath("appstore"), BundleId = "com.apple.AppStore" },
                new AppInfo { Id = 12, Name = "iTunes", IconPath = GetScaledIconPath("itunes"), BundleId = "com.apple.MobileStore" },
                new AppInfo { Id = 13, Name = "Game Center", IconPath = GetScaledIconPath("gamecenter"), BundleId = "com.apple.gamecenter" },
                new AppInfo { Id = 14, Name = "Settings", IconPath = GetScaledIconPath("settings"), BundleId = "com.apple.Preferences", ComponentType = typeof(Apps.Settings.SettingsApp) }
            };

            // Page 2
            var page2 = new List<AppInfo>
            {
                new AppInfo { Id = 15, Name = "Reminders", IconPath = GetScaledIconPath("reminders"), BundleId = "com.apple.reminders" },
                new AppInfo { Id = 16, Name = "Videos", IconPath = GetScaledIconPath("videos"), BundleId = "com.apple.videos" },
                new AppInfo { Id = 17, Name = "UIKit Demo", IconPath = GetScaledIconPath("settings"), BundleId = "com.xenthio.uikitdemo", ComponentType = typeof(Apps.UIKit.UIKitDemoApp) },
                new AppInfo { Id = 18, Name = "Alarm", IconPath = GetScaledIconPath("clock"), BundleId = "com.xenthio.alarm", ComponentType = typeof(Apps.Clock.ClockAlarmEdit) },
                new AppInfo { Id = 19, Name = "Note", IconPath = GetScaledIconPath("notes"), BundleId = "com.xenthio.note", ComponentType = typeof(Apps.Notes.NotesEditor) },
                new AppInfo { Id = 20, Name = "Compose", IconPath = GetScaledIconPath("mail"), BundleId = "com.xenthio.compose", ComponentType = typeof(Apps.Mail.MailCompose) }
            };

            Pages.Clear();
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
