namespace GarageGenius.Common.Models;

public class ServiceVisit
{
    public int Id { get; set; }
    public DateTime EntryDate { get; set; }
    public string? Notes { get; set; }
    public string? Reason { get; set; }
    public uint KmAtVisit { get; set; }
    public int? CustomerId { get; set; }
    public virtual Customer? Customer { get; set; }
    public int? GarageDeviceId { get; set; }
    public virtual GarageDevice? RecordingDevice { get; set; }
    public virtual ICollection<WorkshopLogEntryImage> Images { get; set; } = new List<WorkshopLogEntryImage>();
    public int VehicleId { get; set; }
    public virtual Vehicle Vehicle { get; set; } = null!;
}