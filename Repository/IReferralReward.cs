using WatchMate_API.Entities;

namespace WatchMate_API.Repository
{
    public interface IReferralReward : IGenericRepository<ReferralReward>
    {
        Task ApplyReferralRewardAsync(string? referralCode, int buyerUserId, int packageId);

    }
}
