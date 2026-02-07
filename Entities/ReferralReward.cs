using System.ComponentModel.DataAnnotations.Schema;

namespace WatchMate_API.Entities
{
    [Table("ReferralReward", Schema = "dbo")]
    public class ReferralReward
    {
        public int Id { get; set; }
        public int ReferrerId { get; set; } // The code owner
        public int ReferredUserId { get; set; } // The new user who used the code

        public int PackageId { get; set; } // Which package was purchased
        public decimal RewardAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
