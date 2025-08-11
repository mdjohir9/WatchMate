using WatchMate_API.DTO;
using WatchMate_API.Entities;
using WatchMate_API.Repository;
using Microsoft.EntityFrameworkCore;

namespace WatchMate_API.Implementation
{
    public class WithdrawRepository : GenericRepository<Withdraw>, IWithdrawRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public WithdrawRepository(ApplicationDbContext dbContext , IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<List<WithdrawDetailDTO>> GetAllWithdrawDetailsAsync()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

            var query = from wd in _dbContext.Withdraw
                        join cei in _dbContext.CustomerInfo on wd.CustomerId equals cei.CustomerId into ceiGroup
                        from cei in ceiGroup.DefaultIfEmpty()
                        join pa in _dbContext.PaymentAccount on wd.PaymentMethodID equals pa.PayAcId into paGroup
                        from pa in paGroup.DefaultIfEmpty()
                        join appuser in _dbContext.Users on wd.ApproveBy equals appuser.UserId into userGroup
                        from appuser in userGroup.DefaultIfEmpty()
                        join rejuser in _dbContext.Users on wd.RejectBy equals rejuser.UserId into RejuserGroup
                        from rejuser in RejuserGroup.DefaultIfEmpty()
                        join upuser in _dbContext.Users on wd.UpdatedBy equals upuser.UserId into UpuserGroup
                        from upuser in UpuserGroup.DefaultIfEmpty()
                        join applyuser in _dbContext.Users on wd.ApplyedBy equals applyuser.UserId into applyuserGroup
                        from applyuser in applyuserGroup.DefaultIfEmpty()
                        orderby wd.WithdrawaID descending
                        select new WithdrawDetailDTO
                        {
                            WithdrawaID = wd.WithdrawaID,
                            AccountNumber = wd.AccountNumber,
                            Amount = wd.Amount,
                            RequestedDate = wd.RequestedDate,
                            IsApproved = wd.IsApproved,
                            TransactionCode = wd.TransactionCode,
                            AdminRemarks = wd.AdminRemarks,  
                            CustommerID = wd.CustomerId,
                            FullName = cei.FullName,
                            CustommerImage = $"{baseUrl}/1111/CustommerImage/{cei.CustmerImage}",
                            CustCardNo = cei.CustCardNo,
                            ApproveAt = wd.ApproveAt,
                            ApproveBy = appuser.Email,
                            RejectBy = rejuser.Email,
                            RejectedAt = wd.RejectAt,
                            UpdateBy = upuser.Email,
                            UpdatedAt = wd.UpdatedAt,
                            ApplyedBy = applyuser.Email,
                            ApplyedAt = wd.ApplyedAt,
                            PaymentMethodType=pa.BankOrWalletName,
                        };

            return await query.ToListAsync();
        }
        public async Task<List<WithdrawDetailDTO>> GetWithdrawDetailsByCustomerIdAsync(int customerId)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

            var query = from wd in _dbContext.Withdraw
                        join cei in _dbContext.CustomerInfo on wd.CustomerId equals cei.CustomerId into ceiGroup
                        from cei in ceiGroup.DefaultIfEmpty()
                        join PayAc in _dbContext.PaymentAccount on wd.PaymentMethodID equals PayAc.PayAcId into PayAcGroup
                        from PayAc in PayAcGroup.DefaultIfEmpty()
                        join user in _dbContext.Users on wd.ApproveBy equals user.UserId into userGroup
                        from user in userGroup.DefaultIfEmpty()
                        join rejuser in _dbContext.Users on wd.RejectBy equals rejuser.UserId into RejuserGroup
                        from rejuser in RejuserGroup.DefaultIfEmpty()
                        join upuser in _dbContext.Users on wd.UpdatedBy equals upuser.UserId into UpuserGroup
                        from upuser in UpuserGroup.DefaultIfEmpty()
                        where wd.CustomerId == customerId
                        orderby wd.WithdrawaID descending
                        select new WithdrawDetailDTO
                        {
                            WithdrawaID = wd.WithdrawaID,
                            AccountNumber = wd.AccountNumber,
                            Amount = wd.Amount,
                            RequestedDate = wd.RequestedDate,
                            IsApproved = wd.IsApproved,
                            TransactionCode = wd.TransactionCode,
                            AdminRemarks = wd.AdminRemarks,
                            CustommerID = wd.CustomerId,
                            FullName = cei.FullName,
                            CustommerImage = $"{baseUrl}/1111/CustommerImage/{cei.CustmerImage}",
                            CustCardNo = cei.CustCardNo,
                            ApproveAt = wd.ApproveAt,
                            ApproveBy = user.Email,
                            RejectBy = rejuser.Email,
                            RejectedAt = wd.RejectAt,
                            UpdateBy = upuser.Email,
                            UpdatedAt = wd.UpdatedAt,
                            BankName=PayAc.BankOrWalletName
                        };

            return await query.ToListAsync();
        }


    }
}
