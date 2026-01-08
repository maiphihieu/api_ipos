using iIpos_core.Data;
using iIpos_core.Dto.Category;
using iIpos_core.Dto.Pagination;
using Microsoft.EntityFrameworkCore;

namespace iIpos_core.Service.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly MyDbContext _context;

        public CategoryService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<CategoryDto>> GetAllAsync(int? storeId, int pageNumber, int pageSize)
        {
            var query = _context.Categories.AsNoTracking();

            if (storeId.HasValue)
            {
                query = query.Where(c => c.StoreId == storeId.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query.OrderBy(c => c.Id)
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            var itemDtos = items.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                LogoUrl = c.LogoUrl,
                StoreId = c.StoreId
            }).ToList();

            return new PagedResult<CategoryDto> { Items = itemDtos, TotalCount = totalCount };
        }
        
        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                LogoUrl = category.LogoUrl,
                StoreId = category.StoreId
            };
        }

        public async Task<CategoryDto> CreateAsync(CreateUpdateCategoryDto createDto)
        {
            var newCategory = new iIpos_core.Data.Category
            {
                Name = createDto.Name,
                LogoUrl = createDto.LogoUrl,
                StoreId = createDto.StoreId,       
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                Id = newCategory.Id,
                Name = newCategory.Name,
                LogoUrl = newCategory.LogoUrl,
                StoreId = newCategory.StoreId,

            };
        }

        public async Task<CategoryDto?> UpdateAsync(int id, CreateUpdateCategoryDto updateDto)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null) return null;

            existingCategory.Name = updateDto.Name;
            existingCategory.LogoUrl = updateDto.LogoUrl;

            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                LogoUrl = existingCategory.LogoUrl,
                StoreId = existingCategory.StoreId,
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
            {
                return false;
            }

            var categoryToDelete = await _context.Categories.FindAsync(id);
            if (categoryToDelete == null) return false;

            _context.Categories.Remove(categoryToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
