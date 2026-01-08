using iIpos_core.Data;
using iIpos_core.Dto.Pagination;
using iIpos_core.Dto.Product;
using iIpos_core.Service.Product;
using Microsoft.EntityFrameworkCore;

namespace iIpos_core.Service.Product
{
    public class ProductService : IProductService
    {
        private readonly MyDbContext _context;

        public ProductService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ProductDto>> GetAllAsync(int? storeId, int? categoryId, int pageNumber, int pageSize)
        {
            var query = _context.Products.AsNoTracking();

            if (storeId.HasValue)
            {
                query = query.Where(p => p.StoreId == storeId.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            query = query.Include(p => p.Category);

            var totalCount = await query.CountAsync();

            var products = await query.OrderBy(p => p.Id)
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                StoreId = p.StoreId
            }).ToList();

            return new PagedResult<ProductDto> { Items = productDtos, TotalCount = totalCount };
        }
        public async Task<ProductDto?> GetByIdAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                StoreId = product.StoreId
            };
        }

        public async Task<ProductDto> CreateAsync(CreateUpdateProductDto createDto)
        {
            var newProduct = new iIpos_core.Data.Product
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Price = createDto.Price,
                ImageUrl = createDto.ImageUrl,
                CategoryId = createDto.CategoryId,
                StoreId = createDto.StoreId
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            var createdProduct = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstAsync(p => p.Id == newProduct.Id);

            return new ProductDto
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                Description = createdProduct.Description,
                Price = createdProduct.Price,
                ImageUrl = createdProduct.ImageUrl,
                CategoryId = createdProduct.CategoryId,
                CategoryName = createdProduct.Category?.Name,
                StoreId = createdProduct.StoreId
            };
        }

        public async Task<ProductDto?> UpdateAsync(int productId, CreateUpdateProductDto updateDto)
        {
            var existingProduct = await _context.Products.FindAsync(productId);

            if (existingProduct == null) return null;

            existingProduct.Name = updateDto.Name;
            existingProduct.Description = updateDto.Description;
            existingProduct.Price = updateDto.Price;
            existingProduct.ImageUrl = updateDto.ImageUrl;
            existingProduct.CategoryId = updateDto.CategoryId;
            existingProduct.StoreId = updateDto.StoreId;

            await _context.SaveChangesAsync();

            var updatedProduct = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstAsync(p => p.Id == productId);

            return new ProductDto
            {
                Id = updatedProduct.Id,
                Name = updatedProduct.Name,
                Description = updatedProduct.Description,
                Price = updatedProduct.Price,
                ImageUrl = updatedProduct.ImageUrl,
                CategoryId = updatedProduct.CategoryId,
                CategoryName = updatedProduct.Category?.Name,
                StoreId = updatedProduct.StoreId
            };
        }

        public async Task<bool> DeleteAsync(int productId)
        {
            var productToDelete = await _context.Products.FindAsync(productId);
            if (productToDelete == null) return false;

            _context.Products.Remove(productToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}