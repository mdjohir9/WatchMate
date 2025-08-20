namespace WatchMate_API.DTO.Customer
{
    public class CustomerPackageDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustmerImage { get; set; }
        public string CustCardNo { get; set; }
        public string FullName { get; set; }
        public string? UsedReferralCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal PackagePrice { get; set; }
        public byte Status { get; set; }
        public string TransctionCode { get; set; }
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public int? PayMethodID { get; set; }
        public string? PMName { get; set; }
    }
}
