using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WatchMate_API.Entities
{
    public class Withdraw
    {
        [Key]
        public int WithdrawaID { get; set; }

        [Required]
        public int PaymentMethodID { get; set; }

        public int? BankId { get; set; }

        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }



        [Required]
        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;

        public bool? IsApproved { get; set; }

        [StringLength(100)]
        public string? TransactionCode { get; set; } 

        [StringLength(500)]
        public string? AdminRemarks { get; set; }

        public DateTime? ApproveAt { get; set; }

        [StringLength(50)]
        public int? ApproveBy { get; set; }   


        public DateTime? RejectAt { get; set; }

        [StringLength(50)]
        public int? RejectBy { get; set; }


        public int? ApplyedBy { get; set; }
        public DateTime? ApplyedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        [Required]
        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public CustomerInfo CustomerInfo { get; set; } = null!;
    }
}
