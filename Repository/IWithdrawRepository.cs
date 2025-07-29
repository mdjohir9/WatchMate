using WatchMate_API.DTO;
using WatchMate_API.Entities;

namespace WatchMate_API.Repository
{
    public interface IWithdrawRepository : IGenericRepository<Withdraw>
    {
   
        Task<List<WithdrawDetailDTO>> GetAllWithdrawDetailsAsync();
        Task<List<WithdrawDetailDTO>> GetWithdrawDetailsByCustomerIdAsync(int customerId);

    }
}
