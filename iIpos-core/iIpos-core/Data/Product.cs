using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iIpos_core.Data
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18, 2)")] 
        public decimal Price { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; } 

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int StoreId { get; set; }
        public Store? Store { get; set; }
    }
}

