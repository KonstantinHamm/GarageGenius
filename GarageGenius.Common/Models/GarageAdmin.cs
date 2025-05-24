namespace GarageGenius.Common.Models;

public class GarageAdmin
{
    public string? CompanyName { get; set; }
    public string? BranchOffice { get; set; }
    public IEnumerable<GarageDevice> Devices { get; set; } = new List<GarageDevice>();
    public DateTime LastLogin { get; set; }
}