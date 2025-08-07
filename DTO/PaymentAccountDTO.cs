namespace WatchMate_API.DTO
{
    public class PaymentAccountDTO
    {
        public int Id { get; set; }
        public string? BankOrWalletName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public bool? IsActive { get; set; }
        public string? Logo { get; set; } 
    }
}
