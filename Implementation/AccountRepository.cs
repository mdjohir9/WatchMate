using WatchMate_API.Entities;
using WatchMate_API.Repository;
using Microsoft.EntityFrameworkCore;

namespace WatchMate_API.Implementation
{
    public class AccountRepository : GenericRepository<AccountBalance>, IAccountRepository 
    {
        private readonly ApplicationDbContext _dbContext;
        public AccountRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        //public async Task<int> GenerateUniqueAccountNumberAsync()
        //{
        //    // Get the last used account number for the current year
        //    int currentYear = DateTime.Now.Year;
        //    var lastAccountNo = await _dbContext.AccountBalance
        //        .Where(a => a.AccountNo.ToString().StartsWith(currentYear.ToString()))
        //        .OrderByDescending(a => a.AccountNo)
        //        .Select(a => a.AccountNo)
        //        .FirstOrDefaultAsync();

        //    // If no account exists for this year, start from 1
        //    int serialNumber = lastAccountNo == 0 ? 1 : (lastAccountNo % 100000) + 1;

        //    // Ensure the account number is 6+ digits: Year + 2-digit serial number
        //    int accountNo = int.Parse(currentYear.ToString() + serialNumber.ToString("D2")); // Serial number padded to 2 digits

        //    return accountNo;
        //}

        public async Task<int> GenerateUniqueAccountNumberAsync()
        {
            // Get the last used account number
            var lastAccountNo = await _dbContext.AccountBalance
                .OrderByDescending(a => a.AccountNo)
                .Select(a => a.AccountNo)
                .FirstOrDefaultAsync();

            // If no account exists, start from 11
            int accountNo = lastAccountNo == 0 ? 11 : lastAccountNo + 1;

            return accountNo;
        }

        public AccountBalance GetAccountInfoCustomerId(int customerId)
        {
            return _dbContext.AccountBalance
                             .FirstOrDefault(c => c.CustomerId == customerId && c.IsActive==1);
        }


    }
}
