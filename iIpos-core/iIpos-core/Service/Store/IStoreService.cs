using iIpos_core.Dto.Menu;
using iIpos_core.Dto.Pagination;
using iIpos_core.Dto.Store;

namespace iIpos_core.Service.StoreService
{
    public interface IStoreService
    {
        Task<List<MenuDto>> GetFullMenuAsync(int storeId);
        Task<PagedResult<StoreDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<StoreDto?> GetByIdAsync(int id);
        Task<StoreDto> CreateAsync(CreateUpdateStoreDto createDto);
        Task<StoreDto?> UpdateAsync(int id, CreateUpdateStoreDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}
