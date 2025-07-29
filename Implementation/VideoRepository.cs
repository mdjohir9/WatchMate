using Microsoft.EntityFrameworkCore;
using WatchMate_API.Entities;
using WatchMate_API.Repository;

namespace WatchMate_API.Implementation
{
    public class VideoRepository : GenericRepository<AdVideo>, IVideoRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VideoRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> SaveVideoAsync(IFormFile videoFile, HttpRequest request)
        {
            if (videoFile == null || videoFile.Length == 0)
                throw new Exception("No video file uploaded.");

            var extension = Path.GetExtension(videoFile.FileName);
            var allowedExtensions = new[] { ".mp4", ".mov", ".avi", ".webm", ".mkv" };

            if (!allowedExtensions.Contains(extension.ToLower()))
                throw new Exception("Unsupported video format.");

            var videoFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");
            if (!Directory.Exists(videoFolder))
                Directory.CreateDirectory(videoFolder);

            var fileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(videoFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await videoFile.CopyToAsync(stream);
            }

            // Return public URL
            return $"{request.Scheme}://{request.Host}/videos/{fileName}";
        }

        public async Task<IEnumerable<object>> GetCustomerAdVideos(int customerId)
        {
            var currentDate = DateTime.Now;

            var videos = await (from cp in _dbContext.CustomerPackage
                                join p in _dbContext.Package on cp.PackageId equals p.PackageId
                                join ci in _dbContext.CustomerInfo on cp.CustomerId equals ci.CustomerId
                                join ac in _dbContext.AccountBalance on cp.CustomerId equals ac.CustomerId
                                join v in _dbContext.AdVideo on 1 equals 1
                                where cp.CustomerId == customerId
                                    && cp.ExpiryDate >= currentDate
                                    && v.IsActive == true
                                    && v.StartDate <= currentDate
                                    && v.EndDate >= currentDate
                                    && (("," + v.PackageIds + ",").Contains("," + cp.PackageId.ToString() + ","))
                                select new
                                {
                                    ci.CustomerId,
                                    ci.CustCardNo,
                                    ci.FullName,
                                    p.PackageName,
                                    v.Title,
                                    v.VideoUrl,
                                    v.RewardPerView,
                                    p.PerAdReward,
                                    p.MinDailyViews,
                                    p.MaxDailyViews,
                                    v.StartDate,
                                    v.EndDate,
                                    cp.PackageId,
                                    ac.Id,
                                    p.PerDayReward,
                                }).ToListAsync();


            return videos;
        }


   
    }
}
