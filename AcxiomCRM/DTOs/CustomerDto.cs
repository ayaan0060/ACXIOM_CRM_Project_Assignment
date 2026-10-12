using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.DTOs
{
    public class CustomerDto
    {
        [Required, MaxLength(100)] public string Name { get; set; } = "";
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required, RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10-digit phone number.")] public string Phone { get; set; } = "";
        public string CompanyName { get; set; } = "";
    }

    public class LeadDto
    {
        [Required] public string Name { get; set; } = "";
        [EmailAddress] public string Email { get; set; } = "";
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10-digit phone number.")] public string Phone { get; set; } = "";
        public string Source { get; set; } = "";
    }

    public class OpportunityDto
    {
        [Required] public string Name { get; set; } = "";
        public int CustomerId { get; set; }
        public string Stage { get; set; } = "Qualification";
        [Range(0.01, double.MaxValue)] public decimal Amount { get; set; }
        [Range(0, 100)] public int Probability { get; set; }
        public DateTime ExpectedCloseDate { get; set; }
    }
}
