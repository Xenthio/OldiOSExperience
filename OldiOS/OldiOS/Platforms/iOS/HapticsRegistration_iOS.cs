using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using OldiOS.Shared.Services;

namespace OldiOS.Services.Platforms.iOS
{
    public static class HapticsRegistration_iOS
    {
        public static MauiAppBuilder UseiOSHaptics(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IHapticService, MauiHapticService_iOS>();
            return builder;
        }
    }
}
