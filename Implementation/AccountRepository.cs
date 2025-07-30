using WatchMate_API.Entities;
using WatchMate_API.Repository;
using Microsoft.EntityFrameworkCore;
using WatchMate_API.DTO;

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

        public AccountBalanceDTO GetAccountBalanceByCustomerId(int customerId)
        {
            var result = (from ab in _dbContext.AccountBalance
                          where ab.CustomerId == customerId && ab.IsActive == 1
                          join cp in _dbContext.CustomerPackage
                              on ab.CustomerId equals cp.CustomerId into abcp
                          from cp in abcp.DefaultIfEmpty()

                              // handle second left join manually to avoid null reference from cp
                          let pkgJoin = (from pkg in _dbContext.Package
                                         where cp != null && pkg.PackageId == cp.PackageId
                                         select pkg).FirstOrDefault()

                          select new AccountBalanceDTO
                          {
                              CustomerId = ab.CustomerId,
                              BalanceAmount = Math.Round(ab.BalanceAmount, 0),
                              PackagePrice = cp != null ? Math.Round(cp.PackagePrice, 0) : 0,
                              PerDayReward = pkgJoin != null ? Math.Round(pkgJoin.PerDayReward ?? 0, 0) : 0
                          }).FirstOrDefault();

            return result;
        }

    }
}
