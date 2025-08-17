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
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var query = from cp in _dbContext.CustomerPackage
                        join ci in _dbContext.CustomerInfo on cp.CustomerId equals ci.CustomerId
                        join p in _dbContext.Package on cp.PackageId equals p.PackageId
                        join pm in _dbContext.PaymentAccount on cp.PayAcId equals pm.PayAcId
                        where !customerId.HasValue || cp.CustomerId == customerId orderby cp.Id descending
                        select new CustomerPackageDTO
                        {
                            Id = cp.Id,
                            CustomerId = ci.CustomerId,
                            CustmerImage =  $"{baseUrl}/1111/CustommerImage/{ci.CustmerImage}",
                            CustCardNo = ci.CustCardNo,
                            FullName = ci.FullName,
                            StartDate = cp.StartDate,
                            ExpiryDate = cp.ExpiryDate,
                            PackagePrice = cp.PackagePrice,
                            Status = cp.Status,
                            TransctionCode = cp.TransctionCode,
                            PackageId = p.PackageId,
                            PackageName = p.PackageName,
                            PayMethodID = cp.PayAcId,
                            PMName = pm.BankOrWalletName,
                            UsedReferralCode = cp.UsedReferralCode,
                        };

            return await query.ToListAsync();
        }


        public async Task<bool> HasCustomerBoughtPackageAsync(int customerId, int packageId)
        {
            return await _dbContext.CustomerPackage
                .AnyAsync(up => up.CustomerId == customerId);
        }
        public async Task DeleteCustomerPackageAsync(int id)
        {

            var entity = await _dbContext.CustomerPackage.FindAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Customer Package not found.");

            if (entity.Status == 1)
            {
                throw new InvalidOperationException($"The Customer Package already approved Its cannot be deleted.");
            }
            _dbContext.CustomerPackage.Remove(entity);  
            await _dbContext.SaveChangesAsync();         
        }

    }
}
