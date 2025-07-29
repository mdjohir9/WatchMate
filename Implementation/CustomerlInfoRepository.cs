using Microsoft.EntityFrameworkCore;
using WatchMate_API.DTO.Customer;
using WatchMate_API.Entities;
using WatchMate_API.Repository;

namespace WatchMate_API.Implementation
{
    public class CustomerlInfoRepository : GenericRepository<CustomerInfo>, ICustomerlInfoRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerlInfoRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GenerateNextCustCardNoAsync()
        {
            var lastCardNo = await _dbContext.CustomerInfo
                .Where(x => x.CustCardNo.StartsWith("WTM"))
                .OrderByDescending(x => x.CustCardNo)
                .Select(x => x.CustCardNo)
                .FirstOrDefaultAsync();

            int nextNumber = 111; // Starting number if no previous records

            if (!string.IsNullOrEmpty(lastCardNo))
            {
                // Extract numeric part from "WTM111"
                var numericPart = lastCardNo.Substring(3);
                if (int.TryParse(numericPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"WTM{nextNumber}";
        }
        public async Task<IEnumerable<CustommerIdAndNameDTO>> GetAllCustommerSummaryAsync(int? customerId)
        {
            var query = _dbContext.CustomerInfo.AsQueryable();

            if (customerId.HasValue)
            {
                query = query.Where(c => c.CustomerId == customerId.Value);
            }
            //else
            //{
            //    query = query.Where(c => (c.IsActive == null || c.IsActive == false));

            //}

            var customers = await query
                .Select(c => new CustommerIdAndNameDTO
                {
                    CustomerID = c.CustomerId,
                    FullName = c.FullName
                })
                .ToListAsync();

            return customers;
        }
    }
}
