namespace WatchMate_API.DTO.Settings
{
    public class AdVideoCreateDTO
    {
        public string Title { get; set; }
        public string? YoutubeVideoUrl { get; set; }
        public bool? IsYouTubeVideo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal RewardPerView { get; set; }
        public bool IsActive { get; set; }

        public string? PackageIds { get; set; }
    }
}
