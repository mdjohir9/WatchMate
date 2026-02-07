using System.ComponentModel.DataAnnotations;

namespace WatchMate_API.DTO.Customer
{
    public class CustomerImageUpdateDTO
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public List<string> ImageBase64List { get; set; }

    }
}
