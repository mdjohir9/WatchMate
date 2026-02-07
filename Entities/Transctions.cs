using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WatchMate_API.Entities
{
    public class Transctions
    {
        [Key]
        public int TransctionID { get; set; }

        [Required]
        public int TransactionType { get; set; }  // e.g., Loan Disbursement, Repayment

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        // Foreign key to Customer
        [Required]
        public int UserId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        public int? PaytMethodID { get; set; }


        public string? Remarks { get; set; }
    }
}
