using WatchMate_API.DTO;
using WatchMate_API.Entities;

namespace WatchMate_API.Repository
{
    public interface IPaymnetAccountRepository : IGenericRepository<PaymentAccount>
    {
        Task<List<PaymentAccountDTO>> GetAllPaymentAccountsAsync();
    }
}
