using System.ComponentModel.DataAnnotations;

namespace WatchMate_API.Entities
{
    public class PaymentMethod
    {
        [Required]
        [Key]
        public int PayMethodID { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public Boolean Status { get; set; }
    }
}
