using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WatchMate_API.DTO;
using WatchMate_API.Entities;
using WatchMate_API.Repository;

namespace WatchMate_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentAccountController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _cache;
        private const int userId = 1;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentAccountController(IUserRepository userRepository, IMemoryCache cache, IUnitOfWork unitOfWork, ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _cache = cache;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        [Route("paymentAccounts")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                string cacheKey = "payment_accounts";
                if (!_cache.TryGetValue(cacheKey, out List<PaymentAccount> cachedList))
                {
                    var list = await _unitOfWork.Payment.GetAllPaymentAccountsAsync();
                    if (list == null || !list.Any())
                        return NotFound(new { StatusCode = 404, message = "No payment accounts found." });

                    _cache.Set(cacheKey, list, TimeSpan.FromMinutes(2));
                    return Ok(new { StatusCode = 200, message = "Success", data = list });
                }

                return Ok(new { StatusCode = 200, message = "Success", data = cachedList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }

        [HttpGet("paymentAccount/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var item = await _unitOfWork.Payment.GetByIdAsync(id);
                if (item == null)
                    return NotFound(new { StatusCode = 404, message = "Payment account not found." });

                return Ok(new { StatusCode = 200, message = "Success", data = item });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] PaymentAccountDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { StatusCode = 400, message = "Invalid input." });

                var entity = new PaymentAccount
                {
                    BankOrWalletName = model.BankOrWalletName,
                    AccountName = model.AccountName,
                    AccountNumber = model.AccountNumber,
                    IsActive = model.IsActive ?? true
                };

                await _unitOfWork.Payment.AddAsync(entity);
                await _unitOfWork.Save();

                _cache.Remove("payment_accounts");

                return Ok(new { StatusCode = 200, message = "Payment account created successfully.", data = entity });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "Error while creating payment account", error = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PaymentAccountDTO model)
        {
            try
            {
                var existing = await _unitOfWork.Payment.GetByIdAsync(id);
                if (existing == null)
                    return NotFound(new { StatusCode = 404, message = "Payment account not found." });

                existing.BankOrWalletName = model.BankOrWalletName;
                existing.AccountName = model.AccountName;
                existing.AccountNumber = model.AccountNumber;
                existing.IsActive = model.IsActive;

                _unitOfWork.Payment.UpdateAsync(existing);
                await _unitOfWork.Save();

                _cache.Remove("payment_accounts");

                return Ok(new { StatusCode = 200, message = "Updated successfully.", data = existing });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "Error while updating payment account", error = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existing = await _unitOfWork.Payment.GetByIdAsync(id);
                if (existing == null)
                    return NotFound(new { StatusCode = 404, message = "Payment account not found." });

                _unitOfWork.Payment.DeleteAsync(id);
                await _unitOfWork.Save();

                _cache.Remove("payment_accounts");

                return Ok(new { StatusCode = 200, message = "Deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "Error while deleting payment account", error = ex.Message });
            }
        }

    }
}
