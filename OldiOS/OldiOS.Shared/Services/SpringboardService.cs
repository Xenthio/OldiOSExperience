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
                new AppInfo { Id = 101, Name = "Phone", IconPath = "_content/OldiOS.Shared/images/icons/phone@2x.png", BundleId = "com.apple.mobilephone" },
                new AppInfo { Id = 102, Name = "Mail", IconPath = "_content/OldiOS.Shared/images/icons/mail@2x.png", BundleId = "com.apple.mobilemail" },
                new AppInfo { Id = 103, Name = "Safari", IconPath = "_content/OldiOS.Shared/images/icons/safari@2x.png", BundleId = "com.apple.mobilesafari" },
                new AppInfo { Id = 104, Name = "Music", IconPath = "_content/OldiOS.Shared/images/icons/music@2x.png", BundleId = "com.apple.music" }
            };

            // Page 1
            var page1 = new List<AppInfo>
            {
                new AppInfo { Id = 1, Name = "Messages", IconPath = "_content/OldiOS.Shared/images/icons/messages@2x.png", BundleId = "com.apple.mobilesms" },
                new AppInfo { Id = 2, Name = "Calendar", IconPath = "_content/OldiOS.Shared/images/icons/calendar@2x.png", BundleId = "com.apple.mobilecal" },
                new AppInfo { Id = 3, Name = "Photos", IconPath = "_content/OldiOS.Shared/images/icons/photos@2x.png", BundleId = "com.apple.mobileslideshow" },
                new AppInfo { Id = 4, Name = "Camera", IconPath = "_content/OldiOS.Shared/images/icons/camera@2x.png", BundleId = "com.apple.camera" },
                new AppInfo { Id = 5, Name = "YouTube", IconPath = "_content/OldiOS.Shared/images/icons/youtube@2x.png", BundleId = "com.google.youtube" },
                new AppInfo { Id = 6, Name = "Stocks", IconPath = "_content/OldiOS.Shared/images/icons/stocks@2x.png", BundleId = "com.apple.stocks" },
                new AppInfo { Id = 7, Name = "Maps", IconPath = "_content/OldiOS.Shared/images/icons/maps@2x.png", BundleId = "com.apple.Maps" },
                new AppInfo { Id = 8, Name = "Weather", IconPath = "_content/OldiOS.Shared/images/icons/weather@2x.png", BundleId = "com.apple.weather" },
                new AppInfo { Id = 9, Name = "Notes", IconPath = "_content/OldiOS.Shared/images/icons/notes@2x.png", BundleId = "com.apple.mobilenotes" },
                new AppInfo { Id = 10, Name = "Clock", IconPath = "_content/OldiOS.Shared/images/icons/clock@2x.png", BundleId = "com.apple.mobiletimer" },
                new AppInfo { Id = 11, Name = "App Store", IconPath = "_content/OldiOS.Shared/images/icons/appstore@2x.png", BundleId = "com.apple.AppStore" },
                new AppInfo { Id = 12, Name = "iTunes", IconPath = "_content/OldiOS.Shared/images/icons/itunes@2x.png", BundleId = "com.apple.MobileStore" },
                new AppInfo { Id = 13, Name = "Game Center", IconPath = "_content/OldiOS.Shared/images/icons/gamecenter@2x.png", BundleId = "com.apple.gamecenter" },
                new AppInfo { Id = 14, Name = "Settings", IconPath = "_content/OldiOS.Shared/images/icons/settings@2x.png", BundleId = "com.apple.Preferences", ComponentType = typeof(Apps.Settings.SettingsApp) }
            };

            // Page 2
            var page2 = new List<AppInfo>
            {
                new AppInfo { Id = 15, Name = "Reminders", IconPath = "_content/OldiOS.Shared/images/icons/reminders@2x.png", BundleId = "com.apple.reminders" },
                new AppInfo { Id = 16, Name = "Videos", IconPath = "_content/OldiOS.Shared/images/icons/videos@2x.png", BundleId = "com.apple.videos" },
                new AppInfo { Id = 17, Name = "UIKit Demo", IconPath = "_content/OldiOS.Shared/images/icons/settings@2x.png", BundleId = "com.xenthio.uikitdemo", ComponentType = typeof(Apps.UIKit.UIKitDemoApp) },
                new AppInfo { Id = 18, Name = "Alarm", IconPath = "_content/OldiOS.Shared/images/icons/clock@2x.png", BundleId = "com.xenthio.alarm", ComponentType = typeof(Apps.Clock.ClockAlarmEdit) },
                new AppInfo { Id = 19, Name = "Note", IconPath = "_content/OldiOS.Shared/images/icons/notes@2x.png", BundleId = "com.xenthio.note", ComponentType = typeof(Apps.Notes.NotesEditor) },
                new AppInfo { Id = 20, Name = "Compose", IconPath = "_content/OldiOS.Shared/images/icons/mail@2x.png", BundleId = "com.xenthio.compose", ComponentType = typeof(Apps.Mail.MailCompose) }
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
