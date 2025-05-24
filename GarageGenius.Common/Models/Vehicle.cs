namespace GarageGenius.Common.Models;

public class Vehicle
{
    public string? LicensePlate { get; set; }
    public string? ChassisNumber { get; set; }
    public IEnumerable<WorkshopLogEntry> Entries { get; set; } = new List<WorkshopLogEntry>();
}