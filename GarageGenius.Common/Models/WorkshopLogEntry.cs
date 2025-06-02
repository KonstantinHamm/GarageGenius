namespace GarageGenius.Common.Models;

public class WorkshopLogEntry
{
    public int Id { get; set; }
    public DateTime EntryDate { get; set; }
    public string? Notes { get; set; }
    public uint KmAtVisit { get; set; }
    
    public int VehicleId { get; set; }
    public virtual Vehicle Vehicle { get; set; } = null!;
    
    public int? GarageDeviceId { get; set; }
    public virtual GarageDevice? RecordingDevice { get; set; }
    
    public virtual ICollection<WorkshopLogEntryImage> Images { get; set; } = new List<WorkshopLogEntryImage>();
}