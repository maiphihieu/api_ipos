using iIpos_core.Dto.Branch;
using iIpos_core.Dto.Pagination;

namespace iIpos_core.Service.Branch
{
    public interface IBranchService
    {

        Task<PagedResult<BranchDto>> GetAllByStoreIdAsync(int? storeId, int pageNumber, int pageSize);
        Task<BranchDto?> GetByIdAsync(int branchId);
        Task<BranchDto> CreateAsync(CreateUpdateBranchDto createDto);
        Task<BranchDto?> UpdateAsync(int branchId, CreateUpdateBranchDto updateDto);
        Task<bool> DeleteAsync(int branchId);
    }
}
