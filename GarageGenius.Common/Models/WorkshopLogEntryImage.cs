namespace GarageGenius.Common.Models;

public class WorkshopLogEntryImage
{
    public int Id { get; set; }
    public string Key { get; set; } = null!; 
    public string Value { get; set; } = null!;
    public int WorkshopLogEntryId { get; set; }
    public virtual ServiceVisit ServiceVisit { get; set; } = null!;
}