using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iIpos_core.Data
{
    [Table("Categories")]
    public class Category
    {
        [Key] 
        public int Id { get; set; }

        [Required] 
        [MaxLength(100)] 
        public string Name { get; set; } = null!;
        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        public int StoreId { get; set; } 
        public Store? Store { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
