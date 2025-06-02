namespace GarageGenius.Common.Models;

public class GarageAdmin
{
    public int Id { get; set; }
    public string? CompanyName { get; set; }
    public string? BranchOffice { get; set; }
    public virtual ICollection<GarageDevice> Devices { get; set; } = new List<GarageDevice>();
    public DateTime LastLogin { get; set; }
}