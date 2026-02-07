using Microsoft.EntityFrameworkCore;
using WatchMate_API.DTO;
using WatchMate_API.Entities;
using WatchMate_API.Repository;

namespace WatchMate_API.Implementation
{
    public class PaymentAccountRepository : GenericRepository<PaymentAccount>, IPaymnetAccountRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PaymentAccountRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<PaymentAccountDTO>> GetAllPaymentAccountsAsync()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            return await _dbContext.PaymentAccount
                .Where(p => p.IsActive == true) // Optional: Filter active accounts only
                .Select(p => new PaymentAccountDTO
                {
                    Id = p.PayAcId,
                    BankOrWalletName = p.BankOrWalletName,
                    AccountName = p.AccountName,
                    AccountNumber = p.AccountNumber,
                    Logo = $"{baseUrl}/logo/" + p.Logo,
                    IsActive = p.IsActive ?? false

                })
                .ToListAsync();
        }
    }
}
