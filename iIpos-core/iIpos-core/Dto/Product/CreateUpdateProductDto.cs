using System.ComponentModel.DataAnnotations;

namespace iIpos_core.Dto.Product
{
    public class CreateUpdateProductDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int StoreId { get; set; }

    }
}
