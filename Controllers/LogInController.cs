using WatchMate_API.DTO;
using WatchMate_API.DTO.users;
using WatchMate_API.Entities;
using WatchMate_API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace WatchMate_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogInController : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly IUnitOfWork _unitOfWork;
        int userId = 1;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LogInController(ApplicationDbContext dbContext, IUnitOfWork unitOfWork, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _cache = cache;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult PostUsers([FromBody] LoginDTO loginDTO)
        {
            try
            {
                if (loginDTO == null)
                {
                    return BadRequest(new { StatusCode = 400, message = "User object is null." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { StatusCode = 400, message = "Invalid model state.", data = ModelState });
                }

                var users = _unitOfWork.Login.GetLoginInfo(loginDTO.UserName, loginDTO.UserPassword);
                var _user = users.FirstOrDefault();

                if (_user == null)
                {
                    return NotFound(new { StatusCode = 404, message = "User not found or invalid credentials." });
                }

                var customer = _unitOfWork.Login.GetCustomerInfoByUserId(_user.UserId);
                var userRole = _unitOfWork.Login.GetUserProfileInfo(_user.UserRoleID);
                var accessToken = _unitOfWork.Login.GenerateJwtToken(_user);

                if (_user.IsAdministrator == null)
                {
                    _user.IsAdministrator = false;
                }

                var userinfo = new LoginInfoDTO
                {
                    UserId = _user.UserId,
                    CompanyId = _user.CompanyId,
                    UserName = ComplexScriptingSystem.ComplexLetters.getEntangledLetters(_user.UserName),
                    UserPassword = ComplexScriptingSystem.ComplexLetters.getEntangledLetters(_user.UserPassword),
                    UserImage = _user.UserImage,
                    UserRoleID = _user.UserRoleID,
                    RoleName = userRole?.UserRoleName,
                    Email = _user.Email,
                    CustomerID = customer?.CustomerId.ToString() ?? "",
                    AdditionalPermissions = _user.AdditionalPermissions,
                    RemovedPermissions = _user.RemovedPermissions,
                    IsAdministrator = _user.IsAdministrator,
                    dataAccessLevel = userRole?.DataAccessLevel.ToString() ?? ""
                };

                HttpContext.Session.SetString("UserId", _user.UserId.ToString());
                HttpContext.Session.SetString("UserName", _user.UserName);

                return Ok(new
                {
                    StatusCode = 200,
                    message = "Login successful.",
                    data = userinfo,
                    AccessToken = accessToken
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    message = "An error occurred.",
                    error = ex.Message
                });
            }
        }

        //test one

        [HttpPost]
        [Route("registration")]
        public async Task<IActionResult> PostUsers([FromBody] RegistrationDTO usersDTO)
        {
            try
            {
                if (usersDTO == null)
                    return BadRequest(new { StatusCode = 400, message = "User object is null." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (usersDTO.NewPassword != usersDTO.ConfirmPassword)
                    return BadRequest(new { StatusCode = 400, message = "New Password and Confirm Password do not match." });

                var userRole = _unitOfWork.Login.GetUserRoleByDataAccessLevel(1);
                if (userRole == null || userRole.UserRoleId == 0)
                    return BadRequest(new { StatusCode = 400, message = "Please set up UserRole." });

                using var transaction = await _unitOfWork.BeginTransactionAsync(); // Start transaction
                var custCardNo = await _unitOfWork.CustomerInfo.GenerateNextCustCardNoAsync();

                try
                {
                    // Step 1: Create user
                    var user = new Users
                    {
                        FirstName = usersDTO.FullName,
                        UserName = ComplexScriptingSystem.ComplexLetters.getTangledLetters(usersDTO.EamilOrPhone),
                        UserPassword = ComplexScriptingSystem.ComplexLetters.getTangledLetters(usersDTO.ConfirmPassword),
                        Email = usersDTO.EamilOrPhone,
                        UserRoleID = userRole.UserRoleId,
                        IsGuestUser = true,
                        IsApprovingAuthority = false,
                        ReferenceID = null,
                        AdditionalPermissions = null,
                        RemovedPermissions = null,
                        DataAccessPermission = null,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CompanyId = "1111"
                    };

                    await _unitOfWork.User.AddAsync(user);
                    await _unitOfWork.Save(); // Save to get UserId

                    // Step 2: Create customer info with saved user ID
                    var customerInfo = new CustomerInfo
                    {
                        FullName = usersDTO.FullName,
                        CustCardNo = custCardNo,
                        EmailOrPhone = usersDTO.EamilOrPhone,
                        Address = usersDTO.Address,
                        DateOfBirth = usersDTO.DateOfBirth,
                        Gender = usersDTO.Gender,
                        NIDOrPassportNumber = usersDTO.NIDOrPassportNumber,
                        UserId = user.UserId
                    };

                  var newCustomer = await _unitOfWork.CustomerInfo.AddAsyncReturn(customerInfo);
                    await _unitOfWork.Save();
                    var account = new AccountBalance
                    {
                        CustomerId = newCustomer.CustomerId, 
                        AccountNo = await _unitOfWork.Account.GenerateUniqueAccountNumberAsync(),
                        BalanceAmount = 0,
                        IsActive = 1,
                        CreatedAt = DateTime.Now
                    };
                    await _unitOfWork.Account.AddAsync(account);
                    await _unitOfWork.Save(); // Save customer info

                    await transaction.CommitAsync(); // ✅ Commit transaction

                    _cache.Remove("users"); // Clear cache if any

                    return Ok(new { StatusCode = 200, message = "User created successfully" });
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync(); // ❌ Rollback if insert fails

                    if (dbEx.InnerException is SqlException sqlEx &&
                        sqlEx.Message.Contains("Cannot insert duplicate key row") &&
                        sqlEx.Message.Contains("IX_Users_UserName"))
                    {
                        return Ok(new
                        {
                            StatusCode = 400,
                            message = "An account with this phone number already exists. Would you like to sign in instead?."
                        });
                    }

                    return StatusCode(500, new { StatusCode = 500, message = "A database error occurred", error = dbEx.Message });
                }
                catch (Exception innerEx)
                {
                    await transaction.RollbackAsync(); // ❌ Rollback on any error
                    return StatusCode(500, new { StatusCode = 500, message = "An unexpected error occurred", error = innerEx.Message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }

    }
}
