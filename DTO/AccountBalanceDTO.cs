namespace WatchMate_API.DTO
{
    public class AccountBalanceDTO
    {
        public int CustomerId { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal PackagePrice { get; set; }
        public decimal? PerDayReward { get; set; }
    }
}
