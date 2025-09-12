using GarageGenius.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace GarageGenius.Common.Data;

public class CustomerRepository(ApplicationDbContext context) : Repository<Customer>(context), ICustomerRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Customer>> GetCustomersWithVisitsAsync()
    {
        return await DbSet
            .Include(c => c.Vehicles)
            .ThenInclude(v => v.ServiceVisits)
            .ToListAsync();

    }

    public async Task UpdateCustomerAsync(Customer customer)
    {
        DbSet.Update(customer);
        await _context.SaveChangesAsync();
    }
}