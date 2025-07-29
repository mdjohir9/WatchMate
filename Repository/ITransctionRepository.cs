using WatchMate_API.Entities;

namespace WatchMate_API.Repository
{
    public interface ITransctionRepository : IGenericRepository<Transctions>
    {
        Task<bool> HasRewardTransactionAsync(int customerId, DateTime date, int transactionType);

    }
}
