using System.ComponentModel.DataAnnotations;

namespace iIpos_core.Dto.Category
{
    public class CreateUpdateCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public string? LogoUrl { get; set; }
        [Required]
        public int StoreId { get; set; }
    }
}
