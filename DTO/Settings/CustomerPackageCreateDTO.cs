using System.ComponentModel.DataAnnotations;

namespace WatchMate_API.DTO.Settings
{
    public class CustomerPackageCreateDTO
    {
        [Required]

        public int CustomerId { get; set; }
        [Required]
        public int UserId { get; set; }

        [Required]
        public int PackageId { get; set; }
        public byte Status { get; set; }
        [Required]
        public int? PayMethodID { get; set; }
        [Required]
        public string? TransctionCode { get; set; }
        public string? UsedReferralCode { get; set; }


    }
}
