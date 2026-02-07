namespace WatchMate_API.DTO
{
    public class TransctionDetailesDTO
    {
        public int CustomerId { get; set; }
        public string? CustommerImage { get; set; }
        public string? FullName { get; set; }
        public string? CustCardNo { get; set; }
        public string? TransactionType { get; set; }
        public decimal? Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Remarks { get; set; }
    }
}
