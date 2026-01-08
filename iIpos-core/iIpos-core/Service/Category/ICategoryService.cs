using iIpos_core.Dto.Category;
using iIpos_core.Dto.Pagination;

namespace iIpos_core.Service.Category
{
    public interface ICategoryService
    {
        Task<PagedResult<CategoryDto>> GetAllAsync(int? storeId, int pageNumber, int pageSize);
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CreateUpdateCategoryDto createDto);
        Task<CategoryDto?> UpdateAsync(int id, CreateUpdateCategoryDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}
