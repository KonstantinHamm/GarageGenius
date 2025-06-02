namespace GarageGenius.Common.Models;

public class Customer
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}