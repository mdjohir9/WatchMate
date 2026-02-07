namespace WatchMate_API.DTO
{
    public class LoginInfoDTO
    {
        public int UserId { get; set; }
        public string? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public byte? Status { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }
        public string? UserImage { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int UserRoleID { get; set; }
        public string? Email { get; set; }
        public bool? IsGuestUser { get; set; }
        public string? CustomerID { get; set; }

        public string? AdditionalPermissions { get; set; }
        public string? RemovedPermissions { get; set; }
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? dataAccessLevel { get; set; }
        public bool? IsAdministrator { get; set; }
        public string? UsedReferralCode { get; set; }
        public int? LoanId { get; set; }

    }
}
