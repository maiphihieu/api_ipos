using iIpos_core.Dto.Pagination;
using iIpos_core.Dto.Product;

namespace iIpos_core.Service.Product
{
    public interface IProductService
    {

        Task<PagedResult<ProductDto>> GetAllAsync(int? storeId, int? categoryId, int pageNumber, int pageSize);
        Task<ProductDto?> GetByIdAsync(int productId);
        Task<ProductDto> CreateAsync(CreateUpdateProductDto createDto);
        Task<ProductDto?> UpdateAsync(int productId, CreateUpdateProductDto updateDto);
        Task<bool> DeleteAsync(int productId);
    }
}
