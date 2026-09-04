using TODOList.Components;
using TODOList.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor(options =>
{
	options.DetailedErrors = true;
});

builder.Services.AddSingleton<DataService>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddSingleton<MusicService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

AppDomain.CurrentDomain.UnhandledException += (_, e) =>
{
	try { Console.Error.WriteLine("UNHANDLED: " + e.ExceptionObject); } catch { }
};

TaskScheduler.UnobservedTaskException += (_, e) =>
{
	try { Console.Error.WriteLine("UNOBSERVED TASK: " + e.Exception); } catch { }
};

app.Run();
