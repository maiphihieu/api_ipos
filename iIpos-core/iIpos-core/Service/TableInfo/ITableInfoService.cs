using iIpos_core.Dto.Pagination;
using iIpos_core.Dto.TableInfo;

namespace iIpos_core.Service.TableInfo
{
    public interface ITableInfoService
    {
        Task<PagedResult<TableInfoDto>> GetAllAsync(int? stooreId,int? branchId, int pageNumber, int pageSize);
        Task<TableInfoDto?> GetByIdAsync(int tableId);
        Task<TableInfoDto> CreateAsync(CreateUpdateTableInfoDto createDto);
        Task<TableInfoDto?> UpdateAsync(int tableId, CreateUpdateTableInfoDto updateDto);
        Task<bool> DeleteAsync(int tableId);
        Task<TableInfoDto?> GetByTokenAsync(string token);
    }
}
