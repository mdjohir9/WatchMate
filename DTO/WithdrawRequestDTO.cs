using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WatchMate_API.DTO
{
    public class WithdrawRequestDTO
    {
        [Required]
        public int PaymentMethodID { get; set; }
        [Required]
        [StringLength(20)]
        public string? AccountNumber { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public int CustommerID { get; set; }
        public int ? UserId { get; set; }
    }
}
