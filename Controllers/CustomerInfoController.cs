using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WatchMate_API.DTO.Customer;
using WatchMate_API.Entities;
using WatchMate_API.Repository;

namespace WatchMate_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerInfoController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerInfoController(IUnitOfWork unitOfWork, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        [Route("custommerIdName")]
        public async Task<IActionResult> GetCustommerIdNameById(int? customerId)
        {
            try
            {

                var result = await _unitOfWork.CustomerInfo.GetAllCustommerSummaryAsync(customerId);

                if (result == null)
                {
                    return NotFound(new { StatusCode = 404, message = "Customer not found!" });
                }

                // Cache the result for future requests


                return Ok(new { StatusCode = 200, message = "Success", data = result });


            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "An error occurred", error = ex.Message });
            }
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                string cacheKey = "all_customers";
                if (!_cache.TryGetValue(cacheKey, out List<CustomerInfo> cachedList))
                {
                    var customers = await _unitOfWork.CustomerInfo.GetAllWithDetailsAsync();
                    if (customers == null || !customers.Any())
                        return NotFound(new { StatusCode = 404, message = "No customer data found." });

                    _cache.Set(cacheKey, customers, TimeSpan.FromMinutes(1));
                    return Ok(new { StatusCode = 200, message = "Success", data = customers });
                }

                return Ok(new { StatusCode = 200, message = "Success", data = cachedList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "Server error", error = ex.Message });
            }
        }

        [HttpGet("customer/{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                string cacheKey = $"customer_{id}";
                if (!_cache.TryGetValue(cacheKey, out CustomerInfo cachedCustomer))
                {
                    var customer = await _unitOfWork.CustomerInfo.GetByIdAsync(id);
                    if (customer == null)
                        return NotFound(new { StatusCode = 404, message = "Customer not found" });

                    _cache.Set(cacheKey, customer, TimeSpan.FromMinutes(1));
                    return Ok(new { StatusCode = 200, message = "Success", data = customer });
                }

                return Ok(new { StatusCode = 200, message = "Success", data = cachedCustomer });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "Error retrieving customer", error = ex.Message });
            }
        }

        [HttpPost("image-update")]
        public async Task<IActionResult> UpdateCustomerImage([FromBody] CustomerImageUpdateDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { StatusCode = 400, message = "Invalid request data" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Find existing customer
                var customer = await _unitOfWork.CustomerInfo.GetByIdAsync(dto.CustomerId);
                if (customer == null)
                    return NotFound(new { StatusCode = 404, message = "Customer not found" });

                // Save new image using your existing method
                var savedImagePath = await _unitOfWork.CustomerInfo.SaveDocumentsListsAsync(
                    dto.ImageBase64List,
                    customer.CustCardNo,
                    "1111",
                    "CustommerImage" // or another document type you prefer
                );

                // Update only the image field
                customer.CustmerImage = savedImagePath;
                _unitOfWork.CustomerInfo.UpdateAsync(customer);
                await _unitOfWork.Save();

                // Clear cache if needed
                _cache.Remove("all_customers");

                return Ok(new { StatusCode = 200, message = "Customer image updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    message = "Error updating customer image",
                    error = ex.Message
                });
            }
        }



        [HttpPost("create")]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerInfoCreateDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { StatusCode = 400, message = "Invalid customer data" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var customer = new CustomerInfo
                {
                    FullName = dto.FullName,
                    CustmerImage = dto.CustmerImage,
                    EmailOrPhone = dto.EmailOrPhone,
                    Address = dto.Address,
                    DateOfBirth = dto.DateOfBirth,
                    Gender = dto.Gender,
                    NIDOrPassportNumber = dto.NIDOrPassportNumber,
                    CreatedAt = DateTime.Now,
                    CreatedBy = 1 // Replace with logged-in user ID
                };

                await _unitOfWork.CustomerInfo.AddAsync(customer);
                await _unitOfWork.Save();
                _cache.Remove("all_customers");

                return Ok(new { StatusCode = 200, message = "Customer created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "Error creating customer", error = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerInfoCreateDTO dto)
        {
            try
            {
                var customer = await _unitOfWork.CustomerInfo.GetByIdAsync(id);
                if (customer == null || customer.Deleted == true)
                    return NotFound(new { StatusCode = 404, message = "Customer not found" });

                customer.FullName = dto.FullName;
                customer.CustmerImage = dto.CustmerImage;
                customer.EmailOrPhone = dto.EmailOrPhone;
                customer.Address = dto.Address;
                customer.DateOfBirth = dto.DateOfBirth;
                customer.Gender = dto.Gender;
                customer.NIDOrPassportNumber = dto.NIDOrPassportNumber;
   
                customer.UpdatedAt = DateTime.Now;
                customer.UpdatedBy = 1; // Replace with logged-in user ID

                await _unitOfWork.CustomerInfo.UpdateAsync(customer);
                await _unitOfWork.Save();

                _cache.Remove("all_customers");
                _cache.Remove($"customer_{id}");

                return Ok(new { StatusCode = 200, message = "Customer updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "Error updating customer", error = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                //var customer = await _unitOfWork.CustomerInfo.GetByIdAsync(id);
                //if (customer == null)
                //    return NotFound(new { StatusCode = 404, message = "Customer not found" });

                //customer.Deleted = true;
                //customer.DeletedAt = DateTime.Now;
                //customer.DeletedBy = 1; // Replace with logged-in user ID

                await _unitOfWork.CustomerInfo.DeleteAsync(id);
                await _unitOfWork.Save();

                _cache.Remove("all_customers");
                _cache.Remove($"customer_{id}");

                return Ok(new { StatusCode = 200, message = "Customer deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, message = "Error deleting customer", error = ex.Message });
            }
        }
    }
}
