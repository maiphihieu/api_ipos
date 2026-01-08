using System.ComponentModel.DataAnnotations;

namespace iIpos_core.Data
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;
        public int Rating { get; set; } // 1 to 5 stars
        public string? NegativeFeedbackTags { get; set; } // "Vệ sinh, Món ăn ngon,..."
        [MaxLength(1000)]
        public string? Comments { get; set; }
        [MaxLength(20)]
        public string? CustomerPhoneNumber { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
