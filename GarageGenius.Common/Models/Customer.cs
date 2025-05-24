namespace GarageGenius.Common.Models;

public class Customer
{
    public string? Name { get; set; }
    public IEnumerable<Vehicle>? Vehicles { get; set; }
}