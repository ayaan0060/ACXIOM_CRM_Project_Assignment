using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class Lead
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [EmailAddress]
        public string Email { get; set; } = "";

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10-digit phone number.")]
        public string Phone { get; set; } = "";

        public string CompanyName { get; set; } = "";
        public string Source { get; set; } = "";
        public string Status { get; set; } = "New";
        public string Priority { get; set; } = "Medium";

        [Range(0, double.MaxValue, ErrorMessage = "Expected value must be positive.")]
        public decimal ExpectedValue { get; set; }

        public string OwnerId { get; set; } = "";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsConverted { get; set; } = false;
    }
}
