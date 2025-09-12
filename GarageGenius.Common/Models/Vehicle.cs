namespace GarageGenius.Common.Models;

public class Vehicle
{
    public int Id { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? LicensePlate { get; set; }
    public string? ChassisNumber { get; set; }
    public int CustomerId { get; set; }
    public string? Notes { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<ServiceVisit> ServiceVisits { get; set; } = new List<ServiceVisit>();
}