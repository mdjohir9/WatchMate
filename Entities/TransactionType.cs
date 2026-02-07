using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WatchMate_API.Entities
{
    public class TransactionType
    {
        [Required]
        [Key]
        public int TransactionTypeID { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(20)]
        public string? Code { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        [Required]
        public bool IsCredit { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [StringLength(50)]
        public string? UpdatedBy { get; set; }
    }
}
