using Microsoft.EntityFrameworkCore;
using WatchMate_API.Entities;
using WatchMate_API.Repository;

namespace WatchMate_API.Implementation
{
    public class WithdrawRuleRepository : GenericRepository<WithdrawRule>, IWithdrawRuleRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public WithdrawRuleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<WithdrawRule> GetActiveRuleAsync()
        {
            var rule = await _dbContext.WithdrawRule
                .Where(r => r.IsActive)
                .OrderByDescending(r => r.RuleId) // latest active
                .FirstOrDefaultAsync();

            // If no rule found, return a default one
            if (rule == null)
            {
                rule = new WithdrawRule
                {
                    DailyLimit = 1,
                    MinAmount = 100,       // default min
                    MaxAmount = 1000,     // default max
                    FeePercentage = 1.0m,  // default 1% charge
                    IsActive = true
                };
            }

            return rule;
        }
    
    }
}