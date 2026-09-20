using AcademyAgain.Components;
using AcademyAgain.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AcademyAgainContext") ?? throw new InvalidOperationException("Connection string 'AcademyAgainContext' not found.");

builder.Services.AddDbContextFactory<AcademyAgainContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.Configure<AcademyAgain.Models.SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<AcademyAgain.Helpers.IEmailSender, AcademyAgain.Helpers.EmailSender>();
builder.Services.AddScoped<AcademyAgain.Helpers.EmailVerificationService>();
builder.Services.AddScoped<AcademyAgain.Services.SessionAdmissionService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/403";
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageData", policy => policy.RequireRole("admin", "moderator"));
    options.AddPolicy("Teaching", policy => policy.RequireRole("admin", "moderator", "teacher"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
});
builder.Services.AddCascadingAuthenticationState();

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    await using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AcademyAgainContext>>().CreateDbContext();
    await db.Database.MigrateAsync();
    await AuthStore.SeedAsync(db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
