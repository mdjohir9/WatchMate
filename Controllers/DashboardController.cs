using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WatchMate_API.Entities;
using WatchMate_API.Repository;

namespace WatchMate_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _cache;
        private const int userId = 1;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardController(IUserRepository userRepository, IMemoryCache cache, IUnitOfWork unitOfWork, ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _cache = cache;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("userAccount/balance/{customerId}")]
        public async Task<IActionResult> GetAccountBalance(int customerId)
        {
            try
            {

                var account =  _unitOfWork.Account.GetAccountInfoCustomerId(customerId);
                if (account == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        message = "Account not found for the customer."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    message = "Account balance retrieved successfully.",
                    data = new
                    {
                        CustomerId = account.CustomerId,
                        Balance = account.BalanceAmount,
                        LastUpdated = account.UpdatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    message = "An error occurred while retrieving account balance.",
                    error = ex.Message
                });
            }
        }


    }
}
