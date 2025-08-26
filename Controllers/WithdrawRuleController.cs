using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using WatchMate_API.DTO;
using WatchMate_API.Entities;
using WatchMate_API.Repository;

namespace WatchMate_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawRuleController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        private readonly IUnitOfWork _unitOfWork;
        int userId = 1;
        public WithdrawRuleController(IUnitOfWork unitOfWork, IMemoryCache cache)
        {

            _cache = cache;
            _unitOfWork = unitOfWork;
        }


        [HttpGet("rules")]
        public async Task<IActionResult> GetAll()
        {
            var rules = await _unitOfWork.WithdrawRule.GetAllAsync();
            if (rules == null)
                return NotFound(new { StatusCode = 404, Message = "Withdraw rule not found." });
            return Ok(new { StatusCode = 200, Data = rules });
        }

        // 👉 GET by ID
        [HttpGet("rule/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rule = await _unitOfWork.WithdrawRule.GetByIdAsync(id);
            if (rule == null)
                return NotFound(new { StatusCode = 404, Message = "Withdraw rule not found." });

            return Ok(new { StatusCode = 200, Data = rule });
        }

        // 👉 CREATE
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] WithdrawRuleDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var rule = new WithdrawRule
            {
                PaymentMethodID = dto.PaymentMethodID,
                RuleTitle = dto.RuleTitle,
                RuleDescription = dto.RuleDescription,
                MinAmount = dto.MinAmount,
                MaxAmount = dto.MaxAmount,
                DailyLimit = dto.DailyLimit,
                FeePercentage = dto.FeePercentage,
                IsActive = dto.IsActive
            };

            await _unitOfWork.WithdrawRule.AddAsync(rule);
            await _unitOfWork.Save();

            return Ok(new { StatusCode = 200, Message = "Withdraw rule created successfully." });
        }

        // 👉 UPDATE
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WithdrawRuleDTO dto)
        {
            var rule = await _unitOfWork.WithdrawRule.GetByIdAsync(id);
            if (rule == null)
                return NotFound(new { StatusCode = 404, Message = "Withdraw rule not found." });
            rule.PaymentMethodID = dto.PaymentMethodID;
            rule.FeePercentage = dto.FeePercentage;
            rule.RuleDescription = dto.RuleDescription;
            rule.MinAmount = dto.MinAmount;
            rule.MaxAmount = dto.MaxAmount;
            rule.FeePercentage = dto.FeePercentage;
            rule.IsActive = dto.IsActive;

            await _unitOfWork.WithdrawRule.UpdateAsync(rule);
            await _unitOfWork.Save();

            return Ok(new { StatusCode = 200, Message = "Withdraw rule updated successfully."});
        }

        // 👉 DELETE
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rule = await _unitOfWork.WithdrawRule.GetByIdAsync(id);
            if (rule == null)
                return NotFound(new { StatusCode = 404, Message = "Withdraw rule not found." });

            await _unitOfWork.WithdrawRule.DeleteAsync(id);
            await _unitOfWork.Save();

            return Ok(new { StatusCode = 200, Message = "Withdraw rule deleted successfully." });
        }
    }
}
