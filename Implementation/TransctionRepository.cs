using Microsoft.EntityFrameworkCore;
using WatchMate_API.DTO;
using WatchMate_API.Entities;
using WatchMate_API.Repository;

namespace WatchMate_API.Implementation
{
    public class TransctionRepository : GenericRepository<Transctions>, ITransctionRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TransctionRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<bool> HasRewardTransactionAsync(int customerId, DateTime date, int transactionType)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _dbContext.Transctions.AnyAsync(t =>
                t.CustomerId == customerId &&
                t.TransactionType == transactionType &&
                t.TransactionDate >= startOfDay &&
                t.TransactionDate < endOfDay);
        }


        public async Task<IEnumerable<TransctionDetailesDTO>> GetTransactionsByCustomerAndDateRangeAsync(int customerId, DateTime fromDate, DateTime toDate)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var result = await (from txs in _dbContext.Transctions
                                join cpi in _dbContext.CustomerInfo on txs.CustomerId equals cpi.CustomerId into cpiJoin
                                from cpi in cpiJoin.DefaultIfEmpty()
                                join typ in _dbContext.TransactionType on txs.TransactionType equals typ.TransactionTypeID into typJoin
                                from typ in typJoin.DefaultIfEmpty()
                            
                                where txs.CustomerId == customerId
                                      && txs.TransactionDate.Date >= fromDate.Date
                                      && txs.TransactionDate.Date <= toDate.Date
                                orderby txs.TransctionID descending
                                select new TransctionDetailesDTO
                                {
                                    CustomerId = txs.CustomerId,
                                    CustommerImage = $"{baseUrl}/1111/CustommerImage/{cpi.CustmerImage}",

                                    FullName = cpi.FullName,
                                    CustCardNo = cpi.CustCardNo,
                                    TransactionType = typ.Name,
                                    Amount = txs.Amount,
                                    TransactionDate = txs.TransactionDate,
                                    Remarks = txs.Remarks
                                }).ToListAsync();

            return result;
        }

        //public async Task<object> GetAdminDashboardSummaryAsync()
        //{
        //    var totalCustomers = await _dbContext.CustommerPersonnelInfo
        //        .Where(c => c.IsDeleted == false || c.IsDeleted == null)
        //        .CountAsync();

        //    var totalActiveLoans = await _dbContext.Loan
        //        .Where(l => l.LoanStatus == 1)
        //        .CountAsync();

        //    var disbursementAmount = await _dbContext.Transctions
        //        .Where(t => t.TransactionType == 1)
        //        .SumAsync(t => (decimal?)t.Amount) ?? 0;

        //    var repaymentAmount = await _dbContext.Transctions
        //        .Where(t => t.TransactionType == 2)
        //        .SumAsync(t => (decimal?)t.Amount) ?? 0;

        //    return new
        //    {
        //        TotalCustomers = totalCustomers,
        //        TotalActiveLoan = totalActiveLoans,
        //        DisbursementAmount = disbursementAmount,
        //        RepaymentAmount = repaymentAmount
        //    };
        //}
        public async Task<object> GetrepaymentAndDisbursedSummaryAsync(int year)
        {
            var disbursementAmount = await _dbContext.Transctions
                .Where(t => t.TransactionType == 1 && t.TransactionDate.Year == year)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            var repaymentAmount = await _dbContext.Transctions
                .Where(t => t.TransactionType == 2 && t.TransactionDate.Year == year)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            // Monthly Repayment Totals
            var monthlyRepayments = await _dbContext.Transctions
                .Where(t => t.TransactionType == 2 && t.TransactionDate.Year == year)
                .GroupBy(t => t.TransactionDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            // Monthly Disbursement Totals
            var monthlyDisbursements = await _dbContext.Transctions
                .Where(t => t.TransactionType == 1 && t.TransactionDate.Year == year)
                .GroupBy(t => t.TransactionDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            // Combine both into 12-month arrays
            var monthlyRepaymentData = Enumerable.Range(1, 12)
                .Select(month =>
                {
                    var match = monthlyRepayments.FirstOrDefault(x => x.Month == month);
                    return match != null ? match.Total : 0;
                })
                .ToList();

            var monthlyDisbursementData = Enumerable.Range(1, 12)
                .Select(month =>
                {
                    var match = monthlyDisbursements.FirstOrDefault(x => x.Month == month);
                    return match != null ? match.Total : 0;
                })
                .ToList();

            return new
            {
                Year = year,
                DisbursementAmount = disbursementAmount,
                RepaymentAmount = repaymentAmount,
                MonthlyRepaymentAmounts = monthlyRepaymentData,
                MonthlyDisbursementAmounts = monthlyDisbursementData
            };
        }
        public async Task<object> GetAdminDashboardSummaryAsync()
        {
            var totalCustomers = await _dbContext.CustomerInfo
                .Where(c => c.IsActive == false || c.IsActive == null)
                .CountAsync();

            var ActiveCustomerPackage = await _dbContext.CustomerPackage
                .Where(l => l.Status == 1)
                .CountAsync();

            var disbursementAmount = await _dbContext.Transctions
                .Where(t => t.TransactionType == 1)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            var repaymentAmount = await _dbContext.Transctions
                .Where(t => t.TransactionType == 2)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            return new
            {
                TotalCustomers = totalCustomers,
                TotalActivePackage = ActiveCustomerPackage,
                //DisbursementAmount = disbursementAmount,
                //RepaymentAmount = repaymentAmount
            };
        }
        public async Task<object> GetRechargeAndWithdrawChartDataAsync(DateTime selectedDate)
        {
            selectedDate = selectedDate.Date;
            DateTime startDate = selectedDate.AddDays(-6);
            DateTime endDate = selectedDate.AddDays(1);


            var withdrawData = await _dbContext.Withdraw
                .Where(w => w.RequestedDate >= startDate && w.RequestedDate < endDate)
                .GroupBy(w => w.RequestedDate.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(x => x.Amount) })
                .ToListAsync();

            var withdrawSeries = new List<decimal>();
            var labels = new List<string>();

            for (int i = 0; i < 7; i++)
            {
                var date = startDate.AddDays(i);
                labels.Add(date.ToString("ddd")); // Sat, Sun, Mon, etc.

                withdrawSeries.Add(withdrawData.FirstOrDefault(x => x.Date == date)?.Total ?? 0);
            }

            return new
            {
               
                Withdraw = new
                {
                    Data = withdrawSeries,
                    Total = withdrawSeries.Sum()
                }
            };
        }

        //public async Task<object> GetRechargeAndWithdrawChartDataAsync(DateTime selectedDate)
        //{
        //    selectedDate = selectedDate.Date;
        //    DateTime startDate = selectedDate.AddDays(-6);
        //    DateTime endDate = selectedDate.AddDays(1);

        //    var rechargeData = await _dbContext.Recharge
        //        .Where(r => r.RequestedDate >= startDate && r.RequestedDate < endDate)
        //        .GroupBy(r => r.RequestedDate.Date)
        //        .Select(g => new { Date = g.Key, Total = g.Sum(x => x.Amount) })
        //        .ToListAsync();

        //    var withdrawData = await _dbContext.Withdraw
        //        .Where(w => w.RequestedDate >= startDate && w.RequestedDate < endDate)
        //        .GroupBy(w => w.RequestedDate.Date)
        //        .Select(g => new { Date = g.Key, Total = g.Sum(x => x.Amount) })
        //        .ToListAsync();

        //    var rechargeSeries = new List<decimal>();
        //    var withdrawSeries = new List<decimal>();
        //    var labels = new List<string>();

        //    for (int i = 0; i < 7; i++)
        //    {
        //        var date = startDate.AddDays(i);
        //        labels.Add(date.ToString("ddd")); // Sat, Sun, Mon, etc.

        //        rechargeSeries.Add(rechargeData.FirstOrDefault(x => x.Date == date)?.Total ?? 0);
        //        withdrawSeries.Add(withdrawData.FirstOrDefault(x => x.Date == date)?.Total ?? 0);
        //    }

        //    return new
        //    {
        //        Labels = labels,
        //        Recharge = new
        //        {
        //            Data = rechargeSeries,
        //            Total = rechargeSeries.Sum()
        //        },
        //        Withdraw = new
        //        {
        //            Data = withdrawSeries,
        //            Total = withdrawSeries.Sum()
        //        }
        //    };
        //}


    }
}
