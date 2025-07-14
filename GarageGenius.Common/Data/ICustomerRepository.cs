using GarageGenius.Common.Models;

namespace GarageGenius.Common.Data;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetCustomersWithVisitsAsync();
}