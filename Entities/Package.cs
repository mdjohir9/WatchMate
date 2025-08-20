using System.ComponentModel.DataAnnotations;

namespace WatchMate_API.Entities
{
    public class Package
    {
        [Key]
        [Required]
        public int PackageId { get; set; }

        [Required, MaxLength(100)]
        public string? PackageName { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int? ValidityDays { get; set; }

        public int? MaxDailyViews { get; set; }
        public int? MinDailyViews { get; set; }

        public decimal? PerAdReward { get; set; }
        public decimal? PerDayReward { get; set; }
        public decimal? RefBonus { get; set; }

        [Required]
        public byte? Status { get; set; }
        public byte? IsFree { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
    }
}
