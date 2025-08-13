using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WatchMate_API.DTO.Settings;
using WatchMate_API.Entities;
using WatchMate_API.Repository;

namespace WatchMate_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _cache;
        private const int userId = 1;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VideoController(IUserRepository userRepository, IMemoryCache cache, IUnitOfWork unitOfWork, ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _cache = cache;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("video/{id}")]
        public async Task<IActionResult> GetAdVideoById(int id)
        {
            try
            {
                var video = await _unitOfWork.Video.GetByIdAsync(id);
                if (video == null)
                {
                    return NotFound(new { StatusCode = 404, message = "Ad video not found." });
                }

                return Ok(new { StatusCode = 200, message = "Success", data = video });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }
        [HttpGet]
        [Route("videos/{customerId}")]
        public async Task<IActionResult> GetAdVideos(int customerId)
        {
            try
            {
                var videos = await _unitOfWork.Video.GetCustomerAdVideos(customerId);

                if (videos == null || !videos.Any())
                    return NotFound(new { StatusCode = 404, message = "Ad videos not found." });

                return Ok(new { StatusCode = 200, message = "Success", data = videos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }


        [HttpGet]
        [Route("videos")]
        public async Task<IActionResult> GetAdVideos()
        {
            try
            {
                var videos = await _unitOfWork.Video.GetAdVideos();
                if (videos == null || !videos.Any())
                {
                    return NotFound(new { StatusCode = 404, message = "Ad videos not found." });
                }

                return Ok(new { StatusCode = 200, message = "Success", data = videos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }
        //[HttpPost]
        //[Route("videos/reward")]
        //public async Task<IActionResult> AddVideoReward(int customerId, int accountNo, decimal perAdReward)
        //{
        //    try
        //    {
        //        // ✅ Optional: validate if customer exists
        //        var customer = await _unitOfWork.CustomerInfo.GetByIdAsync(customerId);
        //        if (customer == null)
        //            return NotFound(new { StatusCode = 404, message = "Customer not found." });

        //        // ✅ Get AccountBalance record
        //        var accountBalance = await _unitOfWork.Account.GetByIdAsync(accountNo);

        //        if (accountBalance == null)
        //            return NotFound(new { StatusCode = 404, message = "Account balance not found." });

        //        // ✅ Add reward
        //        accountBalance.BalanceAmount += perAdReward;
        //        accountBalance.UpdatedAt = DateTime.Now;

        //        await _unitOfWork.Account.UpdateAsync(accountBalance);


        //        await _unitOfWork.Save();
        //        var transactionRecord = new Transctions
        //        {
        //            TransactionType = 2,
        //            Amount = perAdReward,
        //            TransactionDate = DateTime.UtcNow,
        //            CustomerId = customerId,
        //            Remarks = $"Reward earned from watching advertisements. Customer ID {customerId}"
        //        };

        //        await _unitOfWork.Transaction.AddAsync(transactionRecord);

        //        await _unitOfWork.Save();
        //        return Ok(new { StatusCode = 200, message = "Reward added successfully", rewardAmount = perAdReward, totalBalance = accountBalance.BalanceAmount });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
        //    }
        //}
        [HttpPost]
        [Route("videos/reward")]
        public async Task<IActionResult> AddVideoReward([FromBody] VideoRewardDTO dto)
        {
            try
            {
                // ✅ Validate customer
                var customer = await _unitOfWork.CustomerInfo.GetByIdAsync(dto.CustomerId);
                if (customer == null)
                    return NotFound(new { StatusCode = 404, message = "Customer not found." });

                // ✅ Validate account
                var accountBalance = await _unitOfWork.Account.GetByIdAsync(dto.AccountNo);
                if (accountBalance == null)
                    return NotFound(new { StatusCode = 404, message = "Account balance not found." });
                DateTime today = DateTime.UtcNow.Date;
                bool alreadyRewarded = await _unitOfWork.Transaction.HasRewardTransactionAsync(dto.CustomerId, today, 2);

                if (alreadyRewarded)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        message = "Reward already given for today."
                    });
                }
                accountBalance.BalanceAmount += dto.PerAdReward;
                accountBalance.UpdatedAt = DateTime.Now;

                await _unitOfWork.Account.UpdateAsync(accountBalance);
                await _unitOfWork.Save();

                var transactionRecord = new Transctions
                {
                    TransactionType = 2,
                    Amount = dto.PerAdReward,
                    TransactionDate = DateTime.UtcNow,
                    CustomerId = dto.CustomerId,
                    Remarks = $"Reward earned from watching advertisements. Customer ID {dto.CustomerId}"
                };

                await _unitOfWork.Transaction.AddAsync(transactionRecord);
                await _unitOfWork.Save();

                return Ok(new
                {
                    StatusCode = 200,
                    message = "Reward added successfully",
                    rewardAmount = dto.PerAdReward,
                    totalBalance = accountBalance.BalanceAmount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }

        [HttpPost("create")]
        [RequestSizeLimit(524288000)]
        public async Task<IActionResult> Create([FromForm] AdVideoCreateDTO dto, IFormFile videoFile)
        {
            if (!ModelState.IsValid || dto.PackageIds == null || !dto.PackageIds.Any())
                return BadRequest("Invalid data.");

            string videoUrl;
            try
            {
                if (dto.IsYouTubeVideo == true)
                {
                    videoUrl = dto.YoutubeVideoUrl;


                }
                else
                {
                    videoUrl = await _unitOfWork.Video.SaveVideoAsync(videoFile, Request);

                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            var video = new AdVideo
            {
                Title = dto.Title,
                VideoUrl = videoUrl,
                IsYouTubeVideo=dto.IsYouTubeVideo,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate, 
                RewardPerView = dto.RewardPerView,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now,
                PackageIds = dto.PackageIds,
            };

            await _unitOfWork.Video.AddAsync(video);
            await _unitOfWork.Save();
            _cache.Remove("ad_videos");

            return Ok(new { StatusCode = 200, message = "Ad video created with upload.", videoUrl });
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var video = await _unitOfWork.Video.GetByIdAsync(id);
            if (video == null)
                return NotFound(new { StatusCode = 404, message = "Ad video not found." });

            // Delete video file from server
            var videoPath = video.VideoUrl;
            var fileName = Path.GetFileName(new Uri(videoPath).AbsolutePath);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Delete from database
            _unitOfWork.Video.DeleteAsync(id);
            await _unitOfWork.Save();

            // Remove cache
            _cache.Remove("ad_videos");

            return Ok(new { StatusCode = 200, message = "Ad video deleted successfully." });
        }

    }
}
