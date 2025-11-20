using Microsoft.Extensions.Logging;
using OldiOS.Shared.Services;
using OldiOS.Services;
#if IOS
using OldiOS.Services.Platforms.iOS;
#endif

namespace OldiOS
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            // Register iOS system services
            builder.Services.AddSingleton<DisplaySettings>();
            builder.Services.AddSingleton<AnimationService>();
            builder.Services.AddSingleton<BackgroundAppManager>();
            builder.Services.AddSingleton<SpringboardService>();

            // Register native battery service for MAUI
            builder.Services.AddSingleton<INativeBatteryService, MauiNativeBatteryService>();
            builder.Services.AddSingleton<BatteryService>();

            // Register Media Library Service
            builder.Services.AddSingleton<IMediaLibraryService, MauiMediaLibraryService>();
            
            // Register Local File Service for loading local images
            builder.Services.AddSingleton<ILocalFileService, LocalFileService>();

            // Register native haptic service for MAUI
#if IOS
				// iOS platform registration (in Platforms/iOS/HapticsRegistration_iOS.cs)
				builder.UseiOSHaptics();
#else
            builder.Services.AddSingleton<IHapticService, MauiHapticService>();
#endif

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Register WebView Service
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<IWebViewService>(sp =>
                new Services.MauiWebViewService(sp.GetRequiredService<MainPage>()));

            return builder.Build();
        }
    }
}
