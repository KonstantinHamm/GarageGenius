using GarageGenius.Common.Models;

namespace GarageGenius.Common.Data;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<IEnumerable<Customer>> GetCustomersWithVisitsAsync();
    Task UpdateCustomerAsync(Customer customer);
}