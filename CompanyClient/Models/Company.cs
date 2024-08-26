using System.ComponentModel.DataAnnotations;

namespace CompanyClient.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(100, ErrorMessage = "The Name field cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Exchange field is required.")]
        [StringLength(50, ErrorMessage = "The Exchange field cannot be longer than 50 characters.")]
        public string Exchange { get; set; }

        [Required(ErrorMessage = "The Ticker field is required.")]
        [StringLength(10, ErrorMessage = "The Ticker field cannot be longer than 10 characters.")]
        public string Ticker { get; set; }

        [Required(ErrorMessage = "The ISIN field is required.")]
        [RegularExpression(@"^[A-Z]{2}[A-Z0-9]{9}[0-9]$", ErrorMessage = "The ISIN field must be in the format of two letters followed by nine alphanumeric characters and a final digit.")]
        [StringLength(12, ErrorMessage = "The ISIN field must be exactly 12 characters long.")]
        public string Isin { get; set; }

        [Required(ErrorMessage = "The Website field is required.")]
        [Url(ErrorMessage = "The Website field must be a valid URL.")]
        [StringLength(200, ErrorMessage = "The Website field cannot be longer than 200 characters.")]
        public string Website { get; set; }
    }
}
