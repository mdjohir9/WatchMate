using WatchMate_API.DTO;
using WatchMate_API.Entities;
using WatchMate_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Loan_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class WithdrawController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        private readonly IUnitOfWork _unitOfWork;
        int userId = 1;
        public WithdrawController(IUnitOfWork unitOfWork, IMemoryCache cache)
        {

            _cache = cache;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("withdraw/{id}")]
        public async Task<IActionResult> GetWithdrawById(int id)
        {
            try
            {
                var result = await _unitOfWork.Withdraw.GetByIdAsync(id);

                if (result == null)
                {
                    return NotFound(new { StatusCode = 404, message = "Withdraw requests not found!" });
                }

                return Ok(new { StatusCode = 200, message = "Success", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }
        [HttpGet]
        [Route("withdraw-requests")]
        public async Task<IActionResult> GetAllWithdraws()
        {
   
            try
            {
                var result = await _unitOfWork.Withdraw.GetAllWithdrawDetailsAsync();

                if (result == null || !result.Any())
                {
                    return NotFound(new { StatusCode = 404, message = "Withdraw requests not found!" });
                }

                return Ok(new { StatusCode = 200, message = "Success", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }
        [HttpGet]
        [Route("withdraw-requests-by-customerId")]
        public async Task<IActionResult> GetAllWithdrawsByCustomerId(int customerId)
        {
            try
            {
                var result = await _unitOfWork.Withdraw.GetWithdrawDetailsByCustomerIdAsync(customerId);

                if (result == null || !result.Any())
                {
                    return NotFound(new { StatusCode = 404, message = "Withdraw requests not found for the customer!" });
                }

                return Ok(new { StatusCode = 200, message = "Success", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> PostWithdrawRequest([FromBody] WithdrawRequestDTO withdrawDto)
        {
            try
            {
                if (withdrawDto == null)
                {
                    return BadRequest("Withdraw request data cannot be null.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 👉 Step 1: Get customer's current balance
                var rule = await _unitOfWork.WithdrawRule.GetActiveRuleAsync();

                if (rule == null)
                {
                    return BadRequest(new { StatusCode = 400, message = "Withdraw rule not configured." });
                }

                // 👉 Step 2: Check amount against rule
                if (withdrawDto.Amount < rule.MinAmount)
                {
                    return BadRequest(new { StatusCode = 400, message = $"Minimum withdraw amount is {rule.MinAmount}." });
                }

                if (withdrawDto.Amount > rule.MaxAmount)
                {
                    return BadRequest(new { StatusCode = 400, message = $"Maximum withdraw amount is {rule.MaxAmount}." });
                }

                // 👉 Step 3: Check daily limit
                if (rule.DailyLimit.HasValue)
                {
                    var today = DateTime.Now;

                    var todayTotal = await _unitOfWork.Withdraw.GetTodayWithdrawTotalAsync(withdrawDto.CustommerID);

                    if (todayTotal + withdrawDto.Amount > rule.DailyLimit.Value)
                    {
                        return BadRequest(new
                        {
                            StatusCode = 400,
                            message = $"Daily withdrawal limit exceeded. Allowed: {rule.DailyLimit}, Already used: {todayTotal}"
                        });
                    }
                }
                var account = _unitOfWork.Account.GetAccountInfoCustomerId(withdrawDto.CustommerID);

                if (account == null)
                {
                    return NotFound(new { StatusCode = 404, message = "Customer Account not found." });
                }
                // 👉 Step 2: Check if balance is enough
                if (account.BalanceAmount < withdrawDto.Amount)
                {
                    return BadRequest(new { StatusCode = 400, message = "Insufficient balance for withdrawal." });
                }
                if (account != null)
                {
                    if (account.BalanceAmount < withdrawDto.Amount)
                    {
                        return BadRequest(new { StatusCode = 400, Message = "Insufficient balance in customer's account." });
                    }

                    account.BalanceAmount -= withdrawDto.Amount;
                    await _unitOfWork.Account.UpdateAsync(account);
                }
                else
                {
                    return BadRequest(new { StatusCode = 400, Message = "Customer not found." });
                }

           

                // 👉 Step 3: If balance is enough, create the withdrawal request
                var withdrawRequest = new Withdraw
                {
                    PaymentMethodID = withdrawDto.PaymentMethodID,
                    AccountNumber = withdrawDto.AccountNumber,
                    Amount = withdrawDto.Amount,
                    RequestedDate = DateTime.Now,
                    CustomerId = withdrawDto.CustommerID,
                    ApplyedBy=withdrawDto.UserId,
                    ApplyedAt=DateTime.Now,
                };

                await _unitOfWork.Withdraw.AddAsync(withdrawRequest);
                await _unitOfWork.Save();

                return Ok(new
                {
                    StatusCode = 200,
                    message = "Withdraw request submitted successfully.",
                    WithdrawRequestId = withdrawRequest.WithdrawaID
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateWithdrawRequest(int id, [FromBody] WithdrawRequestDTO withdrawDto)
        {
            try
            {
                if (withdrawDto == null)
                {
                    return BadRequest("Withdraw request data cannot be null.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 👉 Step 1: Retrieve existing withdrawal request
                var existingWithdraw = await _unitOfWork.Withdraw.GetByIdAsync(id);
                if (existingWithdraw == null)
                {
                    return NotFound(new { StatusCode = 404, message = "Withdraw request not found." });
                }

                // 👉 Step 2: Prevent editing if already approved
                if (existingWithdraw.IsApproved == true)
                {
                    return BadRequest(new { StatusCode = 400, message = "Approved withdraw requests cannot be edited." });
                }

                // 👉 Step 3: Recheck balance against the new withdrawal amount
                var account = _unitOfWork.Account.GetAccountInfoCustomerId(withdrawDto.CustommerID);
                if (account == null)
                {
                    return NotFound(new { StatusCode = 404, message = "Customer account not found." });
                }

                if (account.BalanceAmount < withdrawDto.Amount)
                {
                    return BadRequest(new { StatusCode = 400, message = "Insufficient balance for updated withdrawal amount." });
                }

                // 👉 Step 4: Update the withdrawal request
                existingWithdraw.PaymentMethodID = withdrawDto.PaymentMethodID;
                existingWithdraw.AccountNumber = withdrawDto.AccountNumber;
                existingWithdraw.Amount = withdrawDto.Amount;
                existingWithdraw.RequestedDate = DateTime.Now;
                existingWithdraw.CustomerId = withdrawDto.CustommerID;
                existingWithdraw.UpdatedBy = withdrawDto.UserId;
                existingWithdraw.UpdatedAt = DateTime.Now;

                _unitOfWork.Withdraw.UpdateAsync(existingWithdraw);
                await _unitOfWork.Save();

                return Ok(new
                {
                    StatusCode = 200,
                    message = "Withdraw request updated successfully.",
                    WithdrawRequestId = existingWithdraw.WithdrawaID
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteWithdrawRequest(int id)
        {
            try
            {
                // 👉 Step 1: Retrieve the existing withdrawal request
                var existingWithdraw = await _unitOfWork.Withdraw.GetByIdAsync(id);
                if (existingWithdraw == null)
                {
                    return NotFound(new { StatusCode = 404, message = "Withdraw request not found." });
                }

                // 👉 Step 2: Prevent deletion if already approved
                if (existingWithdraw.IsApproved == true)
                {
                    return BadRequest(new { StatusCode = 400, message = "Approved withdraw requests cannot be deleted." });
                }

                // 👉 Step 3: Delete and save changes
                _unitOfWork.Withdraw.DeleteAsync(id);
                await _unitOfWork.Save();

                return Ok(new
                {
                    StatusCode = 200,
                    message = "Withdraw request deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }



        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveWithdrawRequest(int id,int userId,string transctionId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var withdraw = await _unitOfWork.Withdraw.GetByIdAsync(id);
                if (withdraw == null)
                {
                    return NotFound(new { StatusCode = 404, Message = $"Withdraw request with ID {id} not found." });
                }

                if (withdraw.IsApproved==true)
                {
                    return BadRequest(new { StatusCode = 400, Message = "Withdraw request already approved." });
                }

                var customerId = withdraw.CustomerId;

                withdraw.IsApproved = true;
                withdraw.ApproveAt = DateTime.UtcNow;
                withdraw.ApproveBy = userId;
                withdraw.AdminRemarks = "Approved"; 
                withdraw.TransactionCode = transctionId;

                await _unitOfWork.Withdraw.UpdateAsync(withdraw);

                var account = _unitOfWork.Account.GetAccountInfoCustomerId(customerId);

                //if (account != null)
                //{
                //    if (account.BalanceAmount < withdraw.Amount)
                //    {
                //        return BadRequest(new { StatusCode = 400, Message = "Insufficient balance in customer's account." });
                //    }

                //    account.BalanceAmount -= withdraw.Amount;
                //    await _unitOfWork.Account.UpdateAsync(account);
                //}
                //else
                //{
                //    return BadRequest(new { StatusCode = 400, Message = "Customer not found." });
                //}

                var transactionRecord = new Transctions
                {
                    TransactionType = 4, // Assuming 4 = Withdraw
                    Amount = withdraw.Amount,
                    TransactionDate = DateTime.UtcNow,
                    CustomerId = customerId,
                    PaytMethodID = withdraw.PaymentMethodID,
                    Remarks = $"Withdraw approved for request ID {withdraw.WithdrawaID}"
                };

                await _unitOfWork.Transaction.AddAsync(transactionRecord);

                await _unitOfWork.Save();
                await transaction.CommitAsync();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Withdraw approved, account updated, and transaction recorded."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while approving the withdraw.",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectWithdrawRequest(int id, int userId, string remarks)
        {
            try
            {
                var withdraw = await _unitOfWork.Withdraw.GetByIdAsync(id);
                if (withdraw == null)
                {
                    return NotFound(new { StatusCode = 404, Message = $"Withdraw request with ID {id} not found." });
                }

                // Already approved? Can't reject anymore
                if (withdraw.IsApproved == true)
                {
                    return BadRequest(new { StatusCode = 400, Message = "Withdraw request already approved." });
                }

                // Already rejected? No need to process again
                if (withdraw.IsApproved == false && withdraw.RejectAt != null)
                {
                    return BadRequest(new { StatusCode = 400, Message = "Withdraw request already rejected." });
                }

                // 👉 Refund logic (return money to customer balance)
                var account = _unitOfWork.Account.GetAccountInfoCustomerId(withdraw.CustomerId);
                if (account == null)
                {
                    return NotFound(new { StatusCode = 404, Message = "Customer account not found." });
                }

                account.BalanceAmount += withdraw.Amount; // refund back
                await _unitOfWork.Account.UpdateAsync(account);

                // 👉 Update withdrawal status
                withdraw.IsApproved = false;
                withdraw.RejectAt = DateTime.UtcNow;
                withdraw.RejectBy = userId;
                withdraw.AdminRemarks = string.IsNullOrEmpty(remarks) ? "Rejected" : remarks;

                await _unitOfWork.Withdraw.UpdateAsync(withdraw);

                // Save everything together
                await _unitOfWork.Save();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Withdraw request rejected and amount refunded successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "An error occurred while rejecting withdraw.", Error = ex.Message });
            }
        }

        //[HttpPut("reject/{id}")]
        //public async Task<IActionResult> RejectWithdrawRequest(int id, int userId, string remarks)
        //{
        //    var withdraw = await _unitOfWork.Withdraw.GetByIdAsync(id);
        //    if (withdraw == null)
        //    {
        //        return NotFound(new { StatusCode = 404, Message = $"Withdraw request with ID {id} not found." });
        //    }

        //    if (withdraw.IsApproved == true)
        //    {
        //        return BadRequest(new { StatusCode = 400, Message = "Withdraw request already approved." });
        //    }

        //    withdraw.IsApproved = false;
        //    withdraw.RejectAt = DateTime.UtcNow;
        //    withdraw.RejectBy = userId;
        //    withdraw.AdminRemarks = string.IsNullOrEmpty(remarks) ? "Rejected" : remarks;

        //    await _unitOfWork.Withdraw.UpdateAsync(withdraw);
        //    await _unitOfWork.Save();

        //    return Ok(new
        //    {
        //        StatusCode = 200,
        //        Message = "Withdraw request rejected successfully."
        //    });
        //}


    }
}
