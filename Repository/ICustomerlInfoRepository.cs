using WatchMate_API.DTO.Customer;
using WatchMate_API.Entities;

namespace WatchMate_API.Repository
{
    public interface ICustomerlInfoRepository : IGenericRepository<CustomerInfo>
    {
        Task<string> GenerateNextCustCardNoAsync();
        Task<IEnumerable<CustommerIdAndNameDTO>> GetAllCustommerSummaryAsync(int? CustommerId);
        Task<IEnumerable<CustommerDetailesDTO>> GetAllWithDetailsAsync();
        Task<string> GenerateNextReferralCodeAsync();

    }
}
