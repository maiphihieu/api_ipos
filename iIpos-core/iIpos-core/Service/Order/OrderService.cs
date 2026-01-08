using iIpos_core.Data;
using iIpos_core.Dto.Order;
using iIpos_core.Dto.Pagination;
using iIpos_core.Enums;
using iIpos_core.Hub;

using iIpos_core.Service.Order;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ilpos_core.Service
{
    public class OrderService : IOrderService
    {
        private readonly MyDbContext _context;
        private readonly IHubContext<OrderHub> _hubContext;

        public OrderService(MyDbContext context, IHubContext<OrderHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
      
        }

        public async Task<PagedResult<OrderResultDto>> GetAllOrdersAsync(int? storeId, int? branchId, int? tableId, int pageNumber, int pageSize)
        {
            var query = _context.Orders.Include(o => o.Branch).AsQueryable();

            if (storeId.HasValue)
            {
                query = query.Where(o => o.Branch != null && o.Branch.StoreId == storeId.Value);
            }

            if (branchId.HasValue)
            {
                query = query.Where(o => o.BranchId == branchId.Value);
            }
            if (tableId.HasValue)
            {
                query = query.Where(o => o.TableInfoId == tableId.Value);
            }

            query = query.Include(o => o.TableInfo)
                         .Include(o => o.OrderItems)
                             .ThenInclude(oi => oi.Product)
                         .OrderByDescending(o => o.OrderTime)
                         .AsNoTracking();

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            var itemDtos = items.Select(order => new OrderResultDto
            {
                Id = order.Id,
                OrderTime = order.OrderTime,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                TableName = order.TableInfo?.Name,
                BranchId = order.BranchId,
                Items = order.OrderItems.Select(oi => new OrderItemResultDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    Price = oi.PriceAtOrder
                }).ToList()
            }).ToList();

            return new PagedResult<OrderResultDto> { Items = itemDtos, TotalCount = totalCount };
        }

        // Đã cập nhật để chỉ cần orderId
        public async Task<OrderResultDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Where(o => o.Id == orderId)
                .Include(o => o.TableInfo)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (order == null) return null;

            return new OrderResultDto
            {
                Id = order.Id,
                OrderTime = order.OrderTime,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                TableName = order.TableInfo?.Name,
                BranchId = order.BranchId,
                Items = order.OrderItems.Select(oi => new OrderItemResultDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    Price = oi.PriceAtOrder
                }).ToList()
            };
        }

        // Đã cập nhật để nhận DTO (chứa cả branchId)
        public async Task<OrderResultDto> CreateOrderAsync(CreateOrderRequestDto requestDto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var table = await _context.TableInfos.FindAsync(requestDto.TableInfoId);
                if (table == null)
                    throw new KeyNotFoundException($"Table with ID {requestDto.TableInfoId} not found.");

                if (table.BranchId != requestDto.BranchId)
                    throw new InvalidOperationException("Table does not belong to the specified branch.");

                decimal totalAmount = 0;
                var orderItems = new List<OrderItem>();
                var productIds = requestDto.OrderItems.Select(item => item.ProductId).ToList();
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                foreach (var itemDto in requestDto.OrderItems)
                {
                    if (!products.TryGetValue(itemDto.ProductId, out var product))
                        throw new KeyNotFoundException($"Product with ID {itemDto.ProductId} not found.");

                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = itemDto.Quantity,
                        PriceAtOrder = product.Price
                    };
                    orderItems.Add(orderItem);
                    totalAmount += orderItem.PriceAtOrder * orderItem.Quantity;
                }

                var newOrder = new iIpos_core.Data.Order
                {
                    TableInfoId = requestDto.TableInfoId,
                    BranchId = requestDto.BranchId, // Lấy từ DTO
                    OrderTime = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    Status = "Pending",
                    OrderItems = orderItems
                };

                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _hubContext.Clients.Group("Staff").SendAsync("ReceiveNewOrderNotification", new
                {
                    message = $"Bàn '{table.Name}' vừa có đơn hàng mới!",
                    orderId = newOrder.Id
                });

                return await GetOrderByIdAsync(newOrder.Id) ?? throw new InvalidOperationException("Failed to retrieve created order.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Đã cập nhật để chỉ cần orderId
        public async Task<OrderResultDto?> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders
                .Include(o => o.TableInfo)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return null;

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return await GetOrderByIdAsync(orderId);
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}