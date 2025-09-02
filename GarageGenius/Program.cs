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
using Microsoft.AspNetCore.Antiforgery;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, lc) => lc.WriteTo.Console());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GarageGenius API",
        Version = "v1"
    });
});

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddMudServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"] ?? throw new InvalidOperationException();
    options.ClientId = builder.Configuration["Auth0:ClientId"] ?? throw new InvalidOperationException();
    options.Scope = "openid profile email";

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

var auth = builder.Services.AddAuthorizationBuilder();

auth.AddPolicy("OfficeOnly", policy =>
    policy.RequireAuthenticatedUser()
        .RequireRole("office"));

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

var app = builder.Build();

// --- AB HIER NUR NOCH Pipeline/Endpoint-Definition ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "GarageGenius API v1"); });
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

app.UseAntiforgery();

app.MapGet("/antiforgery-token", (IAntiforgery af, HttpContext ctx) =>
    {
        var tokens = af.GetAndStoreTokens(ctx);
        return Results.Ok(new { token = tokens.RequestToken });
    })
    .RequireAuthorization();


GarageGenius.Api.Endpoints.Map(app);

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .RequireAuthorization("OfficeOnly");

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
}).RequireAuthorization();

app.Run();