using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchMate_API.DTO
{
    public class WithdrawRuleDTO
    {
        public int? PaymentMethodID { get; set; }

        [StringLength(200)]
        public string RuleTitle { get; set; } = null!;

        [Column(TypeName = "nvarchar(500)")]
        public string? RuleDescription { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public decimal? DailyLimit { get; set; }
        public decimal? FeePercentage { get; set; }
        public bool IsActive { get; set; }

        public int? UserId { get; set; }
    }
}
