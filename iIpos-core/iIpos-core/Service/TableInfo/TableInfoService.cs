using iIpos_core.Data;
using iIpos_core.Dto.Pagination;
using iIpos_core.Dto.TableInfo;
using iIpos_core.Service.TableInfo;
using Microsoft.EntityFrameworkCore;

namespace ilpos_core.Service
{
    public class TableInfoService : ITableInfoService
    {
        private readonly MyDbContext _context;

        public TableInfoService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<TableInfoDto>> GetAllAsync(int? storeId, int? branchId, int pageNumber, int pageSize)
        {
            var query = _context.TableInfos
                .Include(t => t.Branch)
                .AsNoTracking();

            if (branchId.HasValue)
            {
                query = query.Where(t => t.BranchId == branchId.Value);
            }
            else if (storeId.HasValue)
            {
                query = query.Where(t => t.Branch != null && t.Branch.StoreId == storeId.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(t => t.Name)
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            var itemDtos = items.Select(t => new TableInfoDto
            {
                Id = t.Id,
                Name = t.Name,
                BranchId = t.BranchId,
                StoreId = t.Branch?.StoreId,
                Token = t.Token
            }).ToList();

            return new PagedResult<TableInfoDto> { Items = itemDtos, TotalCount = totalCount };
        }

        public async Task<TableInfoDto?> GetByIdAsync(int tableId)
        {
            var table = await _context.TableInfos
                .Include(t => t.Branch)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == tableId);

            if (table == null) return null;

            return new TableInfoDto
            {
                Id = table.Id,
                Name = table.Name,
                BranchId = table.BranchId,
                StoreId = table.Branch?.StoreId,
                Token = table.Token
            };
        }

        public async Task<TableInfoDto> CreateAsync(CreateUpdateTableInfoDto createDto)
        {
            var newTable = new TableInfo
            {
                Name = createDto.Name,
                BranchId = createDto.BranchId,
                Token = Guid.NewGuid().ToString()
            };
            _context.TableInfos.Add(newTable);
            await _context.SaveChangesAsync();

            var result = await GetByIdAsync(newTable.Id);
            return result!;
        }

        public async Task<TableInfoDto?> UpdateAsync(int tableId, CreateUpdateTableInfoDto updateDto)
        {
            var table = await _context.TableInfos.FindAsync(tableId);
            if (table == null) return null;

            table.Name = updateDto.Name;
            table.BranchId = updateDto.BranchId;
            await _context.SaveChangesAsync();

            var result = await GetByIdAsync(tableId);
            return result;
        }

        public async Task<bool> DeleteAsync(int tableId)
        {
            var hasOrders = await _context.Orders.AnyAsync(o => o.TableInfoId == tableId);
            if (hasOrders)
            {
                return false;
            }

            var table = await _context.TableInfos.FindAsync(tableId);
            if (table == null) return false;

            _context.TableInfos.Remove(table);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TableInfoDto?> GetByTokenAsync(string token)
        {
            var table = await _context.TableInfos
                .Include(t => t.Branch)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Token == token);

            if (table == null) return null;

            return new TableInfoDto
            {
                Id = table.Id,
                Name = table.Name,
                BranchId = table.BranchId,
                StoreId = table.Branch?.StoreId,
                Token = table.Token
            };
        }
    }
}