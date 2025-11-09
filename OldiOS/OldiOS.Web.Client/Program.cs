using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OldiOS.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register iOS system services
builder.Services.AddSingleton<DisplaySettings>();
builder.Services.AddSingleton<AnimationService>();
builder.Services.AddSingleton<BackgroundAppManager>();
builder.Services.AddSingleton<SpringboardService>();
builder.Services.AddSingleton<BatteryService>();

await builder.Build().RunAsync();
