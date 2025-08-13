using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WatchMate_API.Entities
{
    [Index(nameof(UserId), IsUnique = true)]
    [Index(nameof(CustCardNo), IsUnique = true)]

    public class CustomerInfo
    {
        [Key]
        public int CustomerId { get; set; }
        public string? CustCardNo { get; set; }

        // Basic Customer Info
        [MaxLength(100)]
        public string? FullName { get; set; }

        public string? CustmerImage { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Email or phone is required")]
        public string? EmailOrPhone { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(50)]
        public string? NIDOrPassportNumber { get; set; }
        public string ReferralCode { get; set; } = null!;
        public decimal ReferralEarnings { get; set; } = 0;

        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }

        public bool? IsActive { get; set; } = true;

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Users Users { get; set; }
    }
}
