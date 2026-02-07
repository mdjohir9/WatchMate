using Microsoft.EntityFrameworkCore;
using WatchMate_API.Entities;
using WatchMate_API.Repository;

namespace WatchMate_API.Implementation
{
    public class ReferralRewardRepository : GenericRepository<ReferralReward>, IReferralReward
    {
        private readonly ApplicationDbContext _dbContext;
        public ReferralRewardRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        // Additional methods specific to ReferralReward can be added here

        public async Task ApplyReferralRewardAsync(string? referralCode, int buyerUserId, int packageId)
        {
            if (string.IsNullOrWhiteSpace(referralCode))
                return;

      

            var referrer = await _dbContext.CustomerInfo.FirstOrDefaultAsync(u => u.ReferralCode == referralCode);
            if (referrer == null )
                return; // invalid or self-referral

            var accountBalance = await _dbContext.AccountBalance.FirstOrDefaultAsync(u => u.CustomerId == referrer.CustomerId);
            if (accountBalance == null)
                return;

            var package = await _dbContext.Package.FindAsync(packageId);
            if (package == null)
                return;

            decimal rewardAmount = (decimal)package.RefBonus; 

            var reward = new ReferralReward
            {
                ReferrerId = referrer.CustomerId,
                ReferredUserId = buyerUserId,
                PackageId = packageId,
                RewardAmount = rewardAmount
            };

            _dbContext.ReferralReward.Add(reward);
            referrer.ReferralEarnings += rewardAmount;
            accountBalance.BalanceAmount += rewardAmount;

            await _dbContext.SaveChangesAsync();
        }
    }
   
}
