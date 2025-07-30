using Microsoft.EntityFrameworkCore;
using WatchMate_API.DTO.Customer;
using WatchMate_API.Entities;
using WatchMate_API.Migrations;
using WatchMate_API.Repository;

namespace WatchMate_API.Implementation
{
    public class CustomerPackageRepository : GenericRepository<CustomerPackage>, ICustomerPackageRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private static List<PermissionRouteDTO> PermissionList = new List<PermissionRouteDTO>();
        //public List<PermissionRouteDTO> PermissionList { get; set; }
        public CustomerPackageRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<CustomerPackageDTO>> GetCustomerPackageByCustomerId(int? customerId = null)
        {
            var query = from cp in _dbContext.CustomerPackage
                        join ci in _dbContext.CustomerInfo on cp.CustomerId equals ci.CustomerId
                        join p in _dbContext.Package on cp.PackageId equals p.PackageId
                        join pm in _dbContext.PaymentMethod on cp.PayMethodID equals pm.PayMethodID
                        where !customerId.HasValue || cp.CustomerId == customerId
                        select new CustomerPackageDTO
                        {
                            Id = cp.Id,
                            CustomerId = ci.CustomerId,
                            CustmerImage = ci.CustmerImage,
                            CustCardNo = ci.CustCardNo,
                            FullName = ci.FullName,
                            StartDate = cp.StartDate,
                            ExpiryDate = cp.ExpiryDate,
                            PackagePrice = cp.PackagePrice,
                            Status = cp.Status,
                            TransctionCode = cp.TransctionCode,
                            PackageId = p.PackageId,
                            PackageName = p.PackageName,
                            PayMethodID = cp.PayMethodID,
                            PMName = pm.Name
                        };

            return await query.ToListAsync();
        }



    }
}
