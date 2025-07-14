using GarageGenius.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace GarageGenius.Common.Data;

public class CustomerRepository(ApplicationDbContext context) : Repository<Customer>(context), ICustomerRepository
{
    public async Task<IEnumerable<Customer>> GetCustomersWithVisitsAsync()
    {
        return await DbSet.Include(c => c.Visits).ToListAsync();
    }
}