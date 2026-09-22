using AcademyAgain.Components;
using AcademyAgain.Helpers;
using AcademyAgain.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AcademyAgainContext") ?? throw new InvalidOperationException("Connection string 'AcademyAgainContext' not found.");

builder.Services.AddDbContextFactory<AcademyAgainContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddQuickGridEntityFrameworkAdapter();
builder.Services.AddSignalR();

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
    options.AddPolicy("SupportOnly", policy => policy.RequireRole("admin", "moderator", "support"));
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
    await SupportSeeder.SeedAsync(db);
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
app.MapGet("/photo/{userId:int}", async (int userId, IDbContextFactory<AcademyAgainContext> dbFactory, HttpContext http) =>
{
    await using var db = await dbFactory.CreateDbContextAsync();
    var user = await db.Users.AsNoTracking()
        .Where(u => u.user_id == userId)
        .Select(u => new { u.photo, u.role_id, u.linked_id })
        .FirstOrDefaultAsync();
    byte[]? bytes = user?.photo is { Length: > 0 } ? user.photo : null;
    if (bytes is null && user?.linked_id is int linkedId)
    {
        if (user.role_id == 3)
        {
            bytes = await db.Students.AsNoTracking()
                .Where(s => s.stud_id == linkedId)
                .Select(s => s.photo)
                .FirstOrDefaultAsync();
        }
        else if (user.role_id == 2)
        {
            bytes = await db.Teachers.AsNoTracking()
                .Where(t => t.teacher_id == linkedId)
                .Select(t => t.photo)
                .FirstOrDefaultAsync();
        }
    }
    var mime = bytes is { Length: > 0 } ? Photo.Mime(bytes) : null;
    if (mime is null)
    {
        http.Response.Headers.CacheControl = "no-store";
        return Results.NotFound();
    }
    http.Response.Headers.CacheControl = "public, max-age=3600";
    return Results.Bytes(bytes!, mime, lastModified: null, entityTag: null, enableRangeProcessing: false);
});
app.MapHub<AcademyAgain.Components.Support.ChatHub>("/chatHub");
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
