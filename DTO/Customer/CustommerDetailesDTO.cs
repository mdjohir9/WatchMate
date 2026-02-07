namespace WatchMate_API.DTO.Customer
{
    public class CustommerDetailesDTO
    {
        public int CustomerId { get; set; }
        public string? CustCardNo { get; set; }
        public string? CompanyId { get; set; }
        public string? CustmerImage { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public string? EmailOrPhone { get; set; }
        public string? NIDOrPassportNumber { get; set; }
        public bool? IsActive { get; set; }
    }
}
