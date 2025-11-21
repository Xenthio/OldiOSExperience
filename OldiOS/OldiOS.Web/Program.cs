using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OldiOS.Shared.Services;
using OldiOS.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register iOS system services
builder.Services.AddSingleton<DisplaySettings>();
builder.Services.AddSingleton<AnimationService>();
builder.Services.AddSingleton<BackgroundAppManager>();
builder.Services.AddSingleton<SpringboardService>();

// Register null native battery service for web (uses JavaScript API instead)
builder.Services.AddSingleton<INativeBatteryService, NullNativeBatteryService>();
builder.Services.AddSingleton<BatteryService>();

// Register null haptic service for web
builder.Services.AddSingleton<IHapticService, NullHapticService>();

// Register media library service with mock data for web
builder.Services.AddSingleton<IMediaLibraryService, WebMediaLibraryService>();

await builder.Build().RunAsync();
