using WatchMate_API.DTO.Customer;
using WatchMate_API.Entities;
using WatchMate_API.Migrations;

namespace WatchMate_API.Repository
{
    public interface ICustomerPackageRepository :IGenericRepository<CustomerPackage>
    {
        Task<List<CustomerPackageDTO>> GetCustomerPackageByCustomerId(int? customerId);
        Task<bool> HasCustomerBoughtPackageAsync(int customerId, int packageId);
        Task DeleteCustomerPackageAsync(int id);   // specific hard delete

    }
}
