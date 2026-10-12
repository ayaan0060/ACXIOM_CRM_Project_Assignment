using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class Opportunity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [Required]
        public string Stage { get; set; } = "Qualification";

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Range(0, 100, ErrorMessage = "Probability must be between 0 and 100.")]
        public int Probability { get; set; }

        public DateTime ExpectedCloseDate { get; set; }

        public string OwnerId { get; set; } = "";
        public string Notes { get; set; } = "";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
