namespace GarageGenius.Api;

public static class Endpoints
{
    public static void Map(WebApplication app)
    {
        var grp = app.MapGroup("/api/customers").RequireAuthorization("OfficeOnly");

        // grp.MapGet("/", ...);
        // grp.MapPost("/", ...);
    }
}