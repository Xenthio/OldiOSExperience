using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OldiOS.Shared.Services;
using OldiOS.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register iOS system services
builder.Services.AddSingleton<DisplaySettings>();
builder.Services.AddSingleton<AnimationService>();
builder.Services.AddSingleton<BackgroundAppManager>();
builder.Services.AddSingleton<SpringboardService>();

// Register null native battery service for web (uses JavaScript API instead)
builder.Services.AddSingleton<INativeBatteryService, NullNativeBatteryService>();
builder.Services.AddSingleton<BatteryService>();

await builder.Build().RunAsync();
