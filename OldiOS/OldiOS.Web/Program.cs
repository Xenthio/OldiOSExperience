using OldiOS.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
	
	// Disable caching in development to fix hot-reload issues
	app.Use(async (context, next) =>
	{
		context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
		context.Response.Headers["Pragma"] = "no-cache";
		context.Response.Headers["Expires"] = "0";
		await next();
	});
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(
		typeof(OldiOS.Shared._Imports).Assembly,
		typeof(OldiOS.Web.Client._Imports).Assembly);

app.Run();
