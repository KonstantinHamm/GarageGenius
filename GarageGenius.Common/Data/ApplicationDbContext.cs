using GarageGenius.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace GarageGenius.Common.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<GarageAdmin> GarageAdmins { get; set; }
    public DbSet<GarageDevice> GarageDevices { get; set; }
    public DbSet<ServiceVisit> ServiceVisits { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<WorkshopLogEntryImage> WorkshopLogEntryImages { get; set; }
}