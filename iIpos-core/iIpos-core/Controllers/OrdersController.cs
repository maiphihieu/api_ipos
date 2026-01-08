using iIpos_core.Data;
using iIpos_core.Dto.Order;
using iIpos_core.Dto.Payment;
using iIpos_core.Hub;
using iIpos_core.Service.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace ilpos_core.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize]

    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly MyDbContext _context;
        public OrdersController(IOrderService orderService, IHubContext<OrderHub> hubContext, MyDbContext context)
        {
            _orderService = orderService;
            _hubContext = hubContext;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("request-payment")]
        public async Task<IActionResult> RequestPayment([FromBody] PaymentRequestDto request)
        {
            // 1. Tìm bàn dựa vào token
            var table = await _context.TableInfos.FirstOrDefaultAsync(t => t.Token == request.Token);
            if (table == null)
            {
                return NotFound("Table not found.");
            }

            // 2. Tìm đơn hàng "Pending" gần nhất của bàn đó
            var activeOrder = await _context.Orders
                .Where(o => o.TableInfoId == table.Id && o.Status == "Pending")
                .OrderByDescending(o => o.OrderTime)
                .FirstOrDefaultAsync();

            // 3. Nếu không có đơn hàng nào, trả về lỗi
            if (activeOrder == null)
            {
                return BadRequest("Chưa có đơn hàng nào đang hoạt động để thanh toán.");
            }

            // 4. Nếu có, gửi thông báo kèm OrderId
            string message = $"Bàn '{table.Name}' đang gọi thanh toán bằng hình thức: {request.PaymentMethod}!";

            await _hubContext.Clients.Group("Staff").SendAsync("ReceivePaymentRequest", new
            {
                message = message,
                orderId = activeOrder.Id // Gửi kèm ID của đơn hàng
            });

            return Ok(new { success = true, message = "Payment request sent." });
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(
    [FromQuery] int? storeId,
    [FromQuery] int? branchId,
    [FromQuery] int? tableId,
    [FromQuery] int pageNumber = 1,            
    [FromQuery] int pageSize = 15)

        {

            var result = await _orderService.GetAllOrdersAsync(storeId, branchId,tableId, pageNumber, pageSize);
            return Ok(result);
        }


        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetById(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto requestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _orderService.CreateOrderAsync(requestDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPatch("{orderId}/status")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] UpdateOrderStatusDto statusDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedOrder = await _orderService.UpdateOrderStatusAsync(orderId, statusDto.Status);
            if (updatedOrder == null) return NotFound();
            return Ok(updatedOrder);
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> Delete(int orderId)
        {
            var success = await _orderService.DeleteOrderAsync(orderId);
            if (!success) return BadRequest("Cannot delete order.");
            return NoContent();
        }
    }
}