using iIpos_core.Data;
using iIpos_core.Dto.Branch;
using iIpos_core.Dto.Pagination;
using Microsoft.EntityFrameworkCore;

namespace iIpos_core.Service.Branch
{
    public class BranchService : IBranchService
    {
        private readonly MyDbContext _context;

        public BranchService(MyDbContext context) 
        {
            _context = context;
        }

        public async Task<PagedResult<BranchDto>> GetAllByStoreIdAsync(int? storeId, int pageNumber, int pageSize)
        {
            var query = _context.Branches   
                .AsNoTracking();
            if (storeId.HasValue)
            {
                query = query.Where(b => b.StoreId == storeId.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query.OrderBy(b => b.Name)
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            var itemDtos = items.Select(b => new BranchDto
            {
                Id = b.Id,
                Name = b.Name,
                Address = b.Address,
                StoreId = b.StoreId
            }).ToList();

            return new PagedResult<BranchDto> { Items = itemDtos, TotalCount = totalCount };
        }
        public async Task<BranchDto?> GetByIdAsync(int branchId)
        {
            var branch = await _context.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == branchId);
            if (branch == null) return null;
            return new BranchDto
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                StoreId = branch.StoreId
            };
        }

        public async Task<BranchDto> CreateAsync(CreateUpdateBranchDto createDto)
        {
            var newBranch = new iIpos_core.Data.Branch
            {
                Name = createDto.Name,
                Address = createDto.Address,
                StoreId = createDto.StoreId
            };
            _context.Branches.Add(newBranch);
            await _context.SaveChangesAsync();
            return new BranchDto
            {
                Id = newBranch.Id,
                Name = newBranch.Name,
                Address = newBranch.Address,
                StoreId = newBranch.StoreId
            };
        }

        public async Task<BranchDto?> UpdateAsync(int branchId, CreateUpdateBranchDto updateDto)
        {
            var branch = await _context.Branches.FindAsync(branchId);
            if (branch == null) return null;

            branch.Name = updateDto.Name;
            branch.Address = updateDto.Address;
            branch.StoreId = updateDto.StoreId;
            await _context.SaveChangesAsync();
            return new BranchDto
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                StoreId = branch.StoreId
            };
        }

        public async Task<bool> DeleteAsync(int branchId)
        {
            var hasOrders = await _context.Orders.AnyAsync(o => o.BranchId == branchId);
            var hasTables = await _context.TableInfos.AnyAsync(t => t.BranchId == branchId);
            if (hasOrders || hasTables)
            {
                return false;
            }

            var branch = await _context.Branches.FindAsync(branchId);
            if (branch == null) return false;

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}