using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WatchMate_API.DTO.Settings;
using WatchMate_API.Entities;
using WatchMate_API.Implementation;
using WatchMate_API.Repository;

namespace WatchMate_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _cache;
        private const int userId = 1;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PackageController(IUserRepository userRepository, IMemoryCache cache, IUnitOfWork unitOfWork, ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _cache = cache;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet]
        [Route("packages")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetPackages()
        {
            try
            {
                string cacheKey = "packages";
                var PaymentAccount = await _unitOfWork.Payment.GetAllPaymentAccountsAsync();

                if (!_cache.TryGetValue(cacheKey, out List<Package> cachedPackages))
                {
                    var packages = await _unitOfWork.Package.GetAllAsync();
                    if (packages == null || !packages.Any())
                        return NotFound(new { StatusCode = 404, message = "Packages not found." });

                    var list = packages.ToList();
                    _cache.Set(cacheKey, list, TimeSpan.FromMinutes(1));
                    return Ok(new { StatusCode = 200, message = "Success", data = list , paymentAccount = PaymentAccount });
                }

                return Ok(new { StatusCode = 200, message = "Success", data = cachedPackages, paymentAccount = PaymentAccount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("package/{id}")]
        public async Task<IActionResult> GetPackage(int id)
        {
            try
            {
                string cacheKey = $"package_{id}";
                if (!_cache.TryGetValue(cacheKey, out Package cachedPackage))
                {
                    var result = await _unitOfWork.Package.GetByIdAsync(id);
                    if (result == null)
                        return NotFound(new { StatusCode = 404, message = "Package not found." });

                    _cache.Set(cacheKey, result, TimeSpan.FromMinutes(1));
                    return Ok(new { StatusCode = 200, message = "Success", data = result });
                }

                return Ok(new { StatusCode = 200, message = "Success", data = cachedPackage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "Error retrieving package", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreatePackage([FromBody] PackageCreateDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { StatusCode = 400, message = "Package object is null." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
      

                var package = new Package
                {
                    PackageName = dto.PackageName,
                    Price = dto.Price,
                    ValidityDays = dto.ValidityDays,
                    MaxDailyViews = dto.MaxDailyViews,
                    PerAdReward = dto.PerAdReward,
                    RefBonus = dto.RefBonus,
                    Status = dto.Status,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId,
                };

                await _unitOfWork.Package.AddAsync(package);
                await _unitOfWork.Save();

                _cache.Remove("packages");

                return Ok(new { StatusCode = 200, message = "Package created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }

        [HttpPut]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdatePackage(int id, [FromBody] PackageCreateDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { StatusCode = 400, message = "Invalid package update data." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingPackage = await _unitOfWork.Package.GetByIdAsync(id);
                if (existingPackage == null || existingPackage.Deleted == true)
                {
                    return NotFound(new { StatusCode = 404, message = "Package not found." });
                }


                existingPackage.PackageName = dto.PackageName;
                existingPackage.Price = dto.Price;
                existingPackage.ValidityDays = dto.ValidityDays;
                existingPackage.MaxDailyViews = dto.MaxDailyViews;
                existingPackage.PerAdReward = dto.PerAdReward;
                existingPackage.RefBonus = dto.RefBonus;
                existingPackage.Status = dto.Status;
                existingPackage.UpdatedAt = DateTime.Now;
                existingPackage.UpdatedBy = userId;

                await _unitOfWork.Package.UpdateAsync(existingPackage);
                await _unitOfWork.Save();

                _cache.Remove("packages");
                _cache.Remove($"package_{id}");

                return Ok(new { StatusCode = 200, message = "Package updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred while updating the package.", error = ex.Message });
            }
        }


        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeletePackage(int id,int userId)
        {
            try
            {
                var check = _unitOfWork.Package.GetByIdAsync(id);
                _unitOfWork.Package.DeleteAsync(id);
                await _unitOfWork.Save();
                _cache.Remove("packages");
                _cache.Remove($"package_{id}");

                return Ok(new { StatusCode = 200, message = "Package deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "Failed to delete package", error = ex.Message });
            }
        }
    }
}
