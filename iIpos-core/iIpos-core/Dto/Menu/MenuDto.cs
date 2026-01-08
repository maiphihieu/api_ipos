using iIpos_core.Dto.Product;

namespace iIpos_core.Dto.Menu
{
    public class MenuDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
