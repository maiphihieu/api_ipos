using iIpos_core.Data;
using iIpos_core.Dto.Menu;
using iIpos_core.Dto.Pagination;
using iIpos_core.Dto.Product;
using iIpos_core.Dto.Store;
using Microsoft.EntityFrameworkCore;

namespace iIpos_core.Service.StoreService
{
    public class StoreService : IStoreService
    {
        private readonly MyDbContext _context;
        public StoreService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuDto>> GetFullMenuAsync(int storeId)
        {
            var categoriesWithProducts = await _context.Categories
                .Where(c => c.StoreId == storeId)
                .Include(c => c.Products) 
                .OrderBy(c => c.Id)
                .AsNoTracking()
                .ToListAsync();

            var result = categoriesWithProducts.Select(c => new MenuDto
            {
                CategoryId = c.Id,
                CategoryName = c.Name,
                Products = c.Products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryId = p.CategoryId,
                    StoreId = p.StoreId
                    
                }).ToList()
            }).ToList();

            return result;
        }

        public async Task<PagedResult<StoreDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Stores.AsNoTracking();

            var totalCount = await query.CountAsync();

            var items = await query.OrderBy(s => s.Name)
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            var itemDtos = items.Select(s => new StoreDto
            {
                Id = s.Id,
                Name = s.Name,
                ImgStore = s.ImgStore,
            }).ToList();

            return new PagedResult<StoreDto> { Items = itemDtos, TotalCount = totalCount };
        }
        public async Task<StoreDto?> GetByIdAsync(int id)
        {
            var store = await _context.Stores
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (store == null) return null;

            return new StoreDto
            {
                Id = store.Id,
                Name = store.Name,
                ImgStore = store.ImgStore,
            };
        }

        public async Task<StoreDto> CreateAsync(CreateUpdateStoreDto createDto)
        {
            var newStore = new Store
            {
                Name = createDto.Name,
                ImgStore = createDto.ImgStore,
            };

            _context.Stores.Add(newStore);
            await _context.SaveChangesAsync();

            return new StoreDto
            {
                Id = newStore.Id,
                Name = newStore.Name,
                ImgStore = newStore.ImgStore,
            };
        }

        public async Task<StoreDto?> UpdateAsync(int id, CreateUpdateStoreDto updateDto)
        {
            var existingStore = await _context.Stores.FindAsync(id);
            if (existingStore == null) return null;

            existingStore.Name = updateDto.Name;
            existingStore.ImgStore = updateDto.ImgStore;

            await _context.SaveChangesAsync();

            return new StoreDto
            {
                Id = existingStore.Id,
                Name = existingStore.Name,
                ImgStore = existingStore.ImgStore,
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var hasBranches = await _context.Branches.AnyAsync(b => b.StoreId == id);
            var hasProducts = await _context.Products.AnyAsync(p => p.StoreId == id);

            if (hasBranches || hasProducts)
            {
                return false;
            }

            var storeToDelete = await _context.Stores.FindAsync(id);
            if (storeToDelete == null) return false;

            _context.Stores.Remove(storeToDelete);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
