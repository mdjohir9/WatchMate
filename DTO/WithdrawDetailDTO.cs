namespace WatchMate_API.DTO
{
    public class WithdrawDetailDTO
    {
        public int WithdrawaID { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestedDate { get; set; }
        public bool? IsApproved { get; set; }
        public string? TransactionCode { get; set; }
        public string? AdminRemarks { get; set; }
        public int CustommerID { get; set; }

        // From Customer
        public string? FullName { get; set; }
        public string? CustommerImage { get; set; }
        public string? CustCardNo { get; set; }
        public string? PaymentMethodType { get; set; }

        public DateTime? ApproveAt { get; set; }
        public string? ApproveBy { get; set; }
        public string? RejectBy { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdatedAt { get; set; } 
        public string? ApplyedBy { get; set; }
        public DateTime? ApplyedAt { get; set; }
    }
}
