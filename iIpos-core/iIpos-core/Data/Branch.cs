using System.ComponentModel.DataAnnotations;

namespace iIpos_core.Data
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!; 

        [MaxLength(500)]
        public string? Address { get; set; }

        public int StoreId { get; set; } 
        public Store? Store { get; set; }
    }
}
