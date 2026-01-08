using System.ComponentModel.DataAnnotations;

namespace iIpos_core.Dto.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; } = null!;
        public int StoreId { get; set; }
    }
}
