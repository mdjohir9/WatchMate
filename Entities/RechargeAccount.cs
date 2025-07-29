using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchMate_API.Entities
{
    public class RechargeAccount
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int RecPaymentMethodId { get; set; }
        [ForeignKey(nameof(RecPaymentMethodId))]
        public PaymentMethod? PaymentMethod { get; set; }

        [Required]
        public string? BankOrWalletName { get; set; }
        [Required]
        public string? AccountName { get; set; }

        [Required]
        public string? AccountNumber { get; set; }
        [Required]
        public Boolean? IsActive { get; set; }


    }
}
