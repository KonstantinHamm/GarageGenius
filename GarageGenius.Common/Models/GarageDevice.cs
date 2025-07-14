namespace GarageGenius.Common.Models;

public class GarageDevice
{
    public int Id { get; set; }
    public int GarageAdminId { get; set; }
    public virtual GarageAdmin GarageAdmin { get; set; } = null!;
    public string? DeviceName { get; set; }
    public DateTime RegisterDate { get; set; }
    public DateTime LastLogin { get; set; }
    public virtual ICollection<ServiceVisit> WorkshopLogEntries { get; set; } = new List<ServiceVisit>();
}