using Microsoft.EntityFrameworkCore;
using WatchMate_API.Entities;
using WatchMate_API.Repository;

namespace WatchMate_API.Implementation
{
    public class TransctionRepository : GenericRepository<Transctions>, ITransctionRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TransctionRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<bool> HasRewardTransactionAsync(int customerId, DateTime date, int transactionType)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _dbContext.Transctions.AnyAsync(t =>
                t.CustomerId == customerId &&
                t.TransactionType == transactionType &&
                t.TransactionDate >= startOfDay &&
                t.TransactionDate < endOfDay);
        }

    }
}
