using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchMate_API.Entities
{
    public class CustomerPackage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int PackageId { get; set; }
        public decimal PackagePrice { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public byte Status { get; set; }// 1=Active 2=Rejected 3=expired
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
        public string ? TransctionCode { get; set; }
        public string? UsedReferralCode { get; set; }
        // Navigation
        [ForeignKey("CustomerId")]
        public virtual CustomerInfo CustomerInfo { get; set; }

        [ForeignKey("PackageId")]
        public virtual Package Package { get; set; }
        public int? PayAcId { get; set; }
        [ForeignKey("PayAcId")]
        public virtual PaymentAccount? PaymentAccount { get; set; }
    }
}
