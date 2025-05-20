using Auth0.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using GarageGenius.Components;
using GarageGenius.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var fileLogger = new LoggerConfiguration()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 90
    )
    .CreateLogger();

builder.Host.UseSerilog(fileLogger);
builder.Services.AddMudServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"] ?? throw new InvalidOperationException();
    options.ClientId = builder.Configuration["Auth0:ClientId"] ?? throw new InvalidOperationException();
    options.Scope = "openid profile email";
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/Account/Login", async Task (HttpContext httpContext, ILogger<Program> logger, string returnUrl = "/") =>
{
    try
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(returnUrl)
            .Build();

        authenticationProperties.IsPersistent = true;
        await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }
    catch (Exception e)
    {
        logger.LogError(e, "Fehler beim Login.");
        throw;
    }
});

app.MapPost("/logout", async (HttpContext context, ILogger<Program> logger) =>
{
    try
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await context.SignOutAsync(
            Auth0Constants.AuthenticationScheme,
            new AuthenticationProperties { RedirectUri = "/" }
        );
        logger.LogInformation("User wurde erfolgreich ausgeloggt.");
        return Results.Ok();
    }
    catch (Exception ex)
    {
        const string errorMessage = "Error logging out.";
        logger.LogError(ex, errorMessage);
        return Results.Problem(errorMessage);
    }
});

app.Run();