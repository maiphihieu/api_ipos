using iIpos_core.Dto.Order;
using iIpos_core.Dto.Pagination;

namespace iIpos_core.Service.Order
{
    public interface IOrderService
    {
        Task<OrderResultDto> CreateOrderAsync(CreateOrderRequestDto requestDto);
        Task<OrderResultDto?> UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<PagedResult<OrderResultDto>> GetAllOrdersAsync(int? storeId,  int? branchId, int? tableId, int pageNumber, int pageSize);
        Task<OrderResultDto?> GetOrderByIdAsync(int orderId);
        Task<bool> DeleteOrderAsync(int orderId);
    }
}
