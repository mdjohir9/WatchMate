using System.ComponentModel.DataAnnotations;

namespace WatchMate_API.Entities
{
    public class AdVideo
    {
        [Key]
        public int AdVideoId { get; set; }

        [Required, MaxLength(150)]
        public string? Title { get; set; }

        [Required]
        public string? VideoUrl { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal RewardPerView { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? PackageIds { get; set; }
        public bool? IsYouTubeVideo { get; set; }
        public TimeSpan? MinWatchingTime { get; set; }
        public TimeSpan? MaxWatchingTime { get; set; }
    }
}
