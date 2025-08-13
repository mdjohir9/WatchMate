using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WatchMate_API.DTO.Customer;
using WatchMate_API.DTO.Settings;
using WatchMate_API.Entities;
using WatchMate_API.Repository;

namespace WatchMate_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerPackageController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _cache;
        private const int userId = 1;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerPackageController(IUserRepository userRepository, IMemoryCache cache, IUnitOfWork unitOfWork, ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _cache = cache;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("packages")]
        public async Task<IActionResult> GetUserPackages()
        {
            try
            {
                string cacheKey = "user_packages";
                if (!_cache.TryGetValue(cacheKey, out List<CustomerPackage> cachedList))
                {
                    var userPackages = await _unitOfWork.UserPackages.GetAllAsync();
                    if (userPackages == null || !userPackages.Any())
                        return NotFound(new { StatusCode = 404, message = "User packages not found." });

                    var list = userPackages.ToList();
                    _cache.Set(cacheKey, list, TimeSpan.FromMinutes(1));
                    return Ok(new { StatusCode = 200, message = "Success", data = list });
                }

                return Ok(new { StatusCode = 200, message = "Success", data = cachedList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("package/{id}")]
        public async Task<IActionResult> GetUserPackage(int id)
        {
            try
            {
                string cacheKey = $"user_package_{id}";
                if (!_cache.TryGetValue(cacheKey, out CustomerPackage cachedItem))
                {
                    var result = await _unitOfWork.UserPackages.GetByIdAsync(id);
                    if (result == null)
                        return NotFound(new { StatusCode = 404, message = "User package not found." });

                    _cache.Set(cacheKey, result, TimeSpan.FromMinutes(1));
                    return Ok(new { StatusCode = 200, message = "Success", data = result });
                }

                return Ok(new { StatusCode = 200, message = "Success", data = cachedItem });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "Error retrieving user package", error = ex.Message });
            }
        }

        [HttpGet("get-customer-package")]
        public async Task<IActionResult> GetCustomerPackages([FromQuery] int? customerId)
        {
            try
            {
                string cacheKey = $"customer_package_{(customerId.HasValue ? customerId.ToString() : "all")}";
                if (!_cache.TryGetValue(cacheKey, out List<CustomerPackageDTO> cachedData))
                {
                    var result = await _unitOfWork.UserPackages.GetCustomerPackageByCustomerId(customerId);

                    if (result == null || !result.Any())
                        return NotFound(new { StatusCode = 404, message = "Customer package(s) not found." });

                    _cache.Set(cacheKey, result, TimeSpan.FromMinutes(1));
                    return Ok(new { StatusCode = 200, message = "Success", data = result });
                }

                return Ok(new { StatusCode = 200, message = "Success (Cache)", data = cachedData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "Error retrieving customer packages", error = ex.Message });
            }
        }


        [HttpPut("package/approve/{id}")]
        public async Task<IActionResult> ApprovePackageRequest(int id, byte Status, int userId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var packageRequest = await _unitOfWork.UserPackages.GetByIdAsync(id);
                if (packageRequest == null)
                    return NotFound(new { StatusCode = 404, Message = $"Package request with ID {id} not found." });

                if (packageRequest.Status == 1) // already active
                    return BadRequest(new { StatusCode = 400, Message = "Package request already approved/active." });

                if (Status == 2)
                {
                    packageRequest.Status = 2; // Rejected
                    packageRequest.UpdatedAt = DateTime.UtcNow;
                    packageRequest.UpdatedBy = userId;
                    await _unitOfWork.UserPackages.UpdateAsync(packageRequest);

                    await _unitOfWork.Save();
                    await transaction.CommitAsync();

                    return Ok(new { StatusCode = 200, Message = "Package request rejected successfully." });
                }
                if (!string.IsNullOrWhiteSpace(packageRequest.UsedReferralCode))
                {
                    await _unitOfWork.Referral.ApplyReferralRewardAsync(
                        packageRequest.UsedReferralCode,
                        packageRequest.CustomerId,
                        packageRequest.PackageId
                    );
                }


                // ✅ Approval Process
                packageRequest.Status = 1; // Active
                packageRequest.StartDate = DateTime.UtcNow;
                packageRequest.ExpiryDate = DateTime.UtcNow.AddDays(30); 
                packageRequest.UpdatedAt = DateTime.UtcNow;
                packageRequest.UpdatedBy = userId;
                await _unitOfWork.UserPackages.UpdateAsync(packageRequest);


           
                var transactionRecord = new Transctions
                {
                    TransactionType = 4, 
                    Amount = packageRequest.PackagePrice,
                    TransactionDate = DateTime.UtcNow,
                    UserId = packageRequest.CustomerId,
                    PaytMethodID = packageRequest.PayAcId ?? 0,
                    Remarks = $"Package purchase approved for request ID {packageRequest.Id}"
                };
                await _unitOfWork.Transaction.AddAsync(transactionRecord);

                await _unitOfWork.Save();
                await transaction.CommitAsync();

                return Ok(new { StatusCode = 200, Message = "Package approved and activated successfully." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while approving the package request.",
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateUserPackage([FromBody] CustomerPackageCreateDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { StatusCode = 400, message = "User package data is null." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Get the package from the database
                var package = await _unitOfWork.Package.GetByIdAsync(dto.PackageId);
                if (package == null)
                    return NotFound(new { StatusCode = 404, message = "Package not found." });

                var startDate = DateTime.Now;
                var expiryDate = package.ValidityDays.HasValue
                    ? startDate.AddDays(package.ValidityDays.Value)
                    : (DateTime?)null;

                var userPackage = new CustomerPackage
                {
                    CustomerId = dto.CustomerId,
                    PackageId = dto.PackageId,
                    PackagePrice = package.Price,
                    StartDate = startDate,
                    ExpiryDate = (DateTime)expiryDate,
                    Status = 0,
                    CreatedAt = DateTime.Now,
                    CreatedBy = dto.UserId,
                    PayAcId = dto.PayMethodID,
                    TransctionCode =dto.TransctionCode,
                    UsedReferralCode = dto.UsedReferralCode
                };

                await _unitOfWork.UserPackages.AddAsync(userPackage);
                await _unitOfWork.Save();

                _cache.Remove("user_packages");

                return Ok(new { StatusCode = 200, message = "User package created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }


        [HttpPut]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateUserPackage(int id, [FromBody] CustomerPackageCreateDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { StatusCode = 400, message = "Invalid data." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existing = await _unitOfWork.UserPackages.GetByIdAsync(id);
                if (existing == null)
                    return NotFound(new { StatusCode = 404, message = "User package not found." });

                existing.CustomerId = dto.CustomerId;
                existing.PackageId = dto.PackageId;
                existing.StartDate = DateTime.Now;
                existing.Status = dto.Status;
                existing.UpdatedAt = DateTime.Now;
                existing.UpdatedBy=dto.UserId;
                existing.PayAcId = dto.PayMethodID;
                existing.TransctionCode = dto.TransctionCode;
                existing.UsedReferralCode = dto.UsedReferralCode;

                await _unitOfWork.UserPackages.UpdateAsync(existing);
                await _unitOfWork.Save();

                _cache.Remove("user_packages");
                _cache.Remove($"user_package_{id}");

                return Ok(new { StatusCode = 200, message = "User package updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "Update failed", error = ex.Message });
            }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteUserPackage(int id)
        {
            try
            {
                var check = _unitOfWork.UserPackages.GetByIdAsync(id);
                 _unitOfWork.UserPackages.DeleteAsync(id);
                _unitOfWork.Save();
                _cache.Remove("user_packages");
                _cache.Remove($"user_package_{id}");

                return Ok(new { StatusCode = 200, message = "User package deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "Deletion failed", error = ex.Message });
            }
        }
    }
}
