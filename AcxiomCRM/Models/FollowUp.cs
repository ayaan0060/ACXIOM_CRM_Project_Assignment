using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class FollowUp
    {
        public int Id { get; set; }

        [Required]
        public string Subject { get; set; } = "";

        public string RelatedType { get; set; } = "Customer";
        public int RelatedId { get; set; }

        [Required]
        public DateTime FollowUpDate { get; set; }

        public string FollowUpType { get; set; } = "Call";
        public string Status { get; set; } = "Planned";
        public string Notes { get; set; } = "";
        public string AssignedUserId { get; set; } = "";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
