using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WatchMate_API.Entities
{
    [Table("WithdrawRule", Schema = "dbo")]
    public class WithdrawRule
    {
        [Key]
        public int RuleId { get; set; }

        public int? PaymentMethodID { get; set; }

        [StringLength(200)]
        public string RuleTitle { get; set; } = null!;

        [Column(TypeName = "nvarchar(500)")]
        public string? RuleDescription { get; set; }

        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public decimal? DailyLimit { get; set; }
        public decimal? FeePercentage { get; set; }

        public bool IsActive { get; set; } = true;

        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
