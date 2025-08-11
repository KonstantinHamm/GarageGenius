using Auth0.AspNetCore.Authentication;
using GarageGenius.Common.Data;
using Microsoft.EntityFrameworkCore;
using GarageGenius.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Service-Registrierung (ALLE müssen vor .Build() passieren!)
builder.Services.AddEndpointsApiExplorer(); // <-- belassen!
builder.Services.AddSwaggerGen(opt => // <-- belassen!
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GarageGenius API",
        Version = "v1"
    });
});

var fileLogger = new LoggerConfiguration()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 90
    )
    .WriteTo.Console()
    .CreateLogger();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Host.UseSerilog(fileLogger);
builder.Services.AddMudServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain    = builder.Configuration["Auth0:Domain"]  ?? throw new InvalidOperationException();
    options.ClientId  = builder.Configuration["Auth0:ClientId"]?? throw new InvalidOperationException();
    options.Scope     = "openid profile email";

    options.OpenIdConnectEvents = new()
    {
        OnTokenValidated = ctx =>
        {
            if (ctx.Principal?.Identity is not ClaimsIdentity identity) return Task.CompletedTask;
            var roleClaims = ctx.Principal
                .FindAll("https://garagepal.app/roles")
                .ToArray();

            foreach (var rc in roleClaims)
            {
                if (!identity.HasClaim(ClaimTypes.Role, rc.Value))
                    identity.AddClaim(new Claim(ClaimTypes.Role, rc.Value));
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OfficeOnly", policy =>
        policy.RequireAuthenticatedUser()
            .RequireClaim("https://garagepal.app/roles", "office"));
});

builder.Services.ConfigureApplicationCookie(o =>
{
    o.ExpireTimeSpan = TimeSpan.FromDays(7);
    o.SlidingExpiration = true;
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.WebHost.UseStaticWebAssets();

// Wichtig: AB HIER NICHT MEHR builder.Services verwenden!
var app = builder.Build();

// --- AB HIER NUR NOCH Pipeline/Endpoint-Definition ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>  
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GarageGenius API v1");
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.UseAntiforgery();

GarageGenius.Api.Endpoints.Map(app);

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
        return Results.Redirect("/");
    }
    catch (Exception ex)
    {
        const string errorMessage = "Error logging out.";
        logger.LogError(ex, errorMessage);
        return Results.Problem(errorMessage);
    }
});

app.Run();