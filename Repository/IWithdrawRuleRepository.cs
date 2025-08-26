using WatchMate_API.Entities;

namespace WatchMate_API.Repository
{
    public interface IWithdrawRuleRepository:IGenericRepository<WithdrawRule>
    {
        Task<WithdrawRule> GetActiveRuleAsync();
    }
}
