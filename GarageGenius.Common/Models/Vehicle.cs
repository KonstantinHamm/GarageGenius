namespace GarageGenius.Common.Models;

public class Vehicle
{
    public int Id { get; set; }
    public string? LicensePlate { get; set; }
    public string? ChassisNumber { get; set; }
    public int CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<WorkshopLogEntry> Entries { get; set; } = new List<WorkshopLogEntry>();
}