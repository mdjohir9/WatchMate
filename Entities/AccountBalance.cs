using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WatchMate_API.Entities
{
    [Index(nameof(AccountNo), IsUnique = true)] // Ensure AccountNo is unique

    public class AccountBalance
    {
   
            [Key]
            public int Id { get; set; }

            [Required]

            public int AccountNo { get; set; } // Unique account number

            [Required]
            public int CustomerId { get; set; } // Foreign Key to Customer

            [ForeignKey(nameof(CustomerId))]
            public CustomerInfo CustomerInfo { get; set; } = null!;

            [Required]
            [Column(TypeName = "decimal(18,2)")]
            public decimal BalanceAmount { get; set; } // Available funds in account
            [Required]
            public byte? IsActive { get; set; }
            public DateTime? CreatedAt { get; set; }
            public int? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public int? UpdatedBy { get; set; }
            public bool? Deleted { get; set; }
            public DateTime? DeletedAt { get; set; }
            public int? DeletedBy { get; set; }
        }
    
}
