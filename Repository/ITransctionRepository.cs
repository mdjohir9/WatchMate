using WatchMate_API.DTO;
using WatchMate_API.Entities;

namespace WatchMate_API.Repository
{
    public interface ITransctionRepository : IGenericRepository<Transctions>
    {
        Task<bool> HasRewardTransactionAsync(int customerId, DateTime date, int transactionType);


        Task<IEnumerable<TransctionDetailesDTO>> GetTransactionsByCustomerAndDateRangeAsync(int customerId, DateTime fromDate, DateTime toDate);
        //Task<object> GetAdminDashboardSummaryAsync();
        Task<object> GetrepaymentAndDisbursedSummaryAsync(int year);
        Task<object> GetRechargeAndWithdrawChartDataAsync(DateTime selectedDate);
        Task<object> GetAdminDashboardSummaryAsync();

    }
}
