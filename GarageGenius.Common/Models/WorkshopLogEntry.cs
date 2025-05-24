namespace GarageGenius.Common.Models;

public class WorkshopLogEntry
{
    public Vehicle? Vehicle { get; set; }
    public DateTime EntryDate { get; set; }
    public string? Notes { get; set; }
    public GarageDevice? RecordingDevice { get; set; }
    public uint KmAtVisit { get; set; }
    public IDictionary<string, string>? Images { get; set; } = new Dictionary<string, string>();
}