using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10-digit phone number.")]
        public string Phone { get; set; } = "";

        [MaxLength(200)]
        public string Address { get; set; } = "";

        public string CompanyName { get; set; } = "";
        public string Status { get; set; } = "Active";
        public string OwnerId { get; set; } = "";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
