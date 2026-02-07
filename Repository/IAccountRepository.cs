using WatchMate_API.DTO;
using WatchMate_API.Entities;

namespace WatchMate_API.Repository
{
    public interface IAccountRepository : IGenericRepository<AccountBalance> 
    {
        Task<int> GenerateUniqueAccountNumberAsync();
        AccountBalance GetAccountInfoCustomerId(int customerId);


        AccountBalanceDTO GetAccountBalanceByCustomerId(int customerId);

    }
}
