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
                new AppInfo { Id = 101, Name = "Phone", IconPath = "_content/OldiOS.Shared/images/icons/phone.png", BundleId = "com.apple.mobilephone", ComponentType = typeof(Apps.Phone.PhoneApp) },
                new AppInfo { Id = 102, Name = "Mail", IconPath = "_content/OldiOS.Shared/images/icons/mail.png", BundleId = "com.apple.mobilemail", ComponentType = typeof(Apps.Mail.MailApp) },
                new AppInfo { Id = 103, Name = "Safari", IconPath = "_content/OldiOS.Shared/images/icons/safari.png", BundleId = "com.apple.mobilesafari", ComponentType = typeof(Apps.Safari.SafariApp) },
                new AppInfo { Id = 104, Name = "Music", IconPath = "_content/OldiOS.Shared/images/icons/music.png", BundleId = "com.apple.music", ComponentType = typeof(Apps.Music.MusicApp) }
            };

            // Page 1
            var page1 = new List<AppInfo>
            {
                new AppInfo { Id = 1, Name = "Messages", IconPath = "_content/OldiOS.Shared/images/icons/messages.png", BundleId = "com.apple.mobilesms", ComponentType = typeof(Apps.Messages.MessagesApp) },
                new AppInfo { Id = 2, Name = "Calendar", IconPath = "_content/OldiOS.Shared/images/icons/calendar.png", BundleId = "com.apple.mobilecal", ComponentType = typeof(Apps.Calendar.CalendarApp) },
                new AppInfo { Id = 3, Name = "Photos", IconPath = "_content/OldiOS.Shared/images/icons/photos.png", BundleId = "com.apple.mobileslideshow", ComponentType = typeof(Apps.Photos.PhotosApp) },
                new AppInfo { Id = 4, Name = "Camera", IconPath = "_content/OldiOS.Shared/images/icons/camera.png", BundleId = "com.apple.camera", ComponentType = typeof(Apps.Camera.CameraApp) },
                new AppInfo { Id = 5, Name = "YouTube", IconPath = "_content/OldiOS.Shared/images/icons/youtube.png", BundleId = "com.google.youtube", ComponentType = typeof(Apps.YouTube.YouTubeApp) },
                new AppInfo { Id = 6, Name = "Stocks", IconPath = "_content/OldiOS.Shared/images/icons/stocks.png", BundleId = "com.apple.stocks", ComponentType = typeof(Apps.Stocks.StocksApp) },
                new AppInfo { Id = 7, Name = "Maps", IconPath = "_content/OldiOS.Shared/images/icons/maps.png", BundleId = "com.apple.Maps", ComponentType = typeof(Apps.Maps.MapsApp) },
                new AppInfo { Id = 8, Name = "Weather", IconPath = "_content/OldiOS.Shared/images/icons/weather.png", BundleId = "com.apple.weather", ComponentType = typeof(Apps.Weather.WeatherApp) },
                new AppInfo { Id = 9, Name = "Notes", IconPath = "_content/OldiOS.Shared/images/icons/notes.png", BundleId = "com.apple.mobilenotes", ComponentType = typeof(Apps.Notes.NotesApp) },
                new AppInfo { Id = 10, Name = "Clock", IconPath = "_content/OldiOS.Shared/images/icons/clock.png", BundleId = "com.apple.mobiletimer", ComponentType = typeof(Apps.Clock.ClockApp) },
                new AppInfo { Id = 11, Name = "App Store", IconPath = "_content/OldiOS.Shared/images/icons/appstore.png", BundleId = "com.apple.AppStore", ComponentType = typeof(Apps.AppStore.AppStoreApp) },
                new AppInfo { Id = 12, Name = "iTunes", IconPath = "_content/OldiOS.Shared/images/icons/itunes.png", BundleId = "com.apple.MobileStore", ComponentType = typeof(Apps.iTunes.ITunesApp) },
                new AppInfo { Id = 13, Name = "Game Center", IconPath = "_content/OldiOS.Shared/images/icons/gamecenter.png", BundleId = "com.apple.gamecenter", ComponentType = typeof(Apps.GameCenter.GameCenterApp) },
                new AppInfo { Id = 14, Name = "Settings", IconPath = "_content/OldiOS.Shared/images/icons/settings.png", BundleId = "com.apple.Preferences", ComponentType = typeof(Apps.Settings.SettingsApp) }
            };

            // Page 2
            var page2 = new List<AppInfo>
            {
                new AppInfo { Id = 15, Name = "Reminders", IconPath = "_content/OldiOS.Shared/images/icons/reminders.png", BundleId = "com.apple.reminders", ComponentType = typeof(Apps.Reminders.RemindersApp) },
                new AppInfo { Id = 16, Name = "Videos", IconPath = "_content/OldiOS.Shared/images/icons/videos.png", BundleId = "com.apple.videos", ComponentType = typeof(Apps.Videos.VideosApp) },
                new AppInfo { Id = 21, Name = "Calculator", IconPath = "_content/OldiOS.Shared/images/icons/calculator.png", BundleId = "com.apple.calculator", ComponentType = typeof(Apps.Calculator.CalculatorApp) },
                new AppInfo { Id = 22, Name = "Contacts", IconPath = "_content/OldiOS.Shared/images/icons/contacts.png", BundleId = "com.apple.mobileaddressbook", ComponentType = typeof(Apps.Contacts.ContactsApp) },
                new AppInfo { Id = 23, Name = "FaceTime", IconPath = "_content/OldiOS.Shared/images/icons/facetime.png", BundleId = "com.apple.facetime", ComponentType = typeof(Apps.FaceTime.FaceTimeApp) },
                new AppInfo { Id = 24, Name = "Voice Memos", IconPath = "_content/OldiOS.Shared/images/icons/voicememos.png", BundleId = "com.apple.VoiceMemos", ComponentType = typeof(Apps.VoiceMemos.VoiceMemosApp) },
                new AppInfo { Id = 25, Name = "Compass", IconPath = "_content/OldiOS.Shared/images/icons/compass.png", BundleId = "com.apple.compass", ComponentType = typeof(Apps.Compass.CompassApp) },
                new AppInfo { Id = 17, Name = "UIKit Demo", IconPath = "_content/OldiOS.Shared/images/icons/settings.png", BundleId = "com.xenthio.uikitdemo", ComponentType = typeof(Apps.UIKit.UIKitDemoApp) },
                new AppInfo { Id = 18, Name = "Alarm", IconPath = "_content/OldiOS.Shared/images/icons/clock.png", BundleId = "com.xenthio.alarm", ComponentType = typeof(Apps.Clock.ClockAlarmEdit) },
                new AppInfo { Id = 19, Name = "Note", IconPath = "_content/OldiOS.Shared/images/icons/notes.png", BundleId = "com.xenthio.note", ComponentType = typeof(Apps.Notes.NotesEditor) },
                new AppInfo { Id = 20, Name = "Compose", IconPath = "_content/OldiOS.Shared/images/icons/mail.png", BundleId = "com.xenthio.compose", ComponentType = typeof(Apps.Mail.MailCompose) }
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
