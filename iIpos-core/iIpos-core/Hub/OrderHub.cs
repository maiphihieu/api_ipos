using iIpos_core.Dto.HubRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace iIpos_core.Hub
{
    public class OrderHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public OrderHub() { }

        public override async Task OnConnectedAsync()
        {
            var userRole = Context.User?.FindFirstValue(ClaimTypes.Role);
            Console.WriteLine($"--> Client connected. Attempting to identify role...");
            if (!string.IsNullOrEmpty(userRole))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userRole);
                Console.WriteLine($"--> User connected and added to group: {userRole}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userRole = Context.User?.FindFirstValue(ClaimTypes.Role);
            if (!string.IsNullOrEmpty(userRole))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userRole);
            }
            await base.OnDisconnectedAsync(exception);
        }

        [AllowAnonymous]
        public async Task JoinTableRoom(string tableToken)
        {
            if (string.IsNullOrEmpty(tableToken)) return;
            await Groups.AddToGroupAsync(Context.ConnectionId, tableToken);
            Console.WriteLine($"--> Client {Context.ConnectionId} joined room: {tableToken}");
        }

        [AllowAnonymous]
        public async Task SendMessageFromCustomer(StaffCallRequest request)
        {
            if (string.IsNullOrEmpty(request?.TableToken) || string.IsNullOrEmpty(request.Message))
                return;

            string displayName = !string.IsNullOrEmpty(request.TableName) ? request.TableName : request.TableToken;

            var chatMessage = new
            {
                TableToken = request.TableToken,
                Text = request.Message,
                Sender = "customer",
                Timestamp = DateTime.UtcNow,
                TableName = displayName
            };

            // === SỬA LỖI 1 (Dòng 60): Dùng GroupExcept ===
            // Gửi cho mọi người trong phòng, TRỪ NGƯỜI GỬI (chính là khách)
            await Clients.GroupExcept(request.TableToken, Context.ConnectionId)
                         .SendAsync("ReceiveMessage", chatMessage);

            // Gửi "rung chuông" (Vẫn giữ nguyên)
            var notification = new
            {
                MessageText = request.Message,
                TableToken = request.TableToken,
                TableName = displayName,
                Timestamp = chatMessage.Timestamp,
                Message = $"Bàn {displayName} có tin nhắn mới: {request.Message}"
            };
            await Clients.Group("Staff").SendAsync("ReceiveStaffCall", notification);
        }

        [Authorize(Roles = "Staff")]
        public async Task SendMessageFromStaff(string tableToken, string message)
        {
            if (string.IsNullOrEmpty(tableToken) || string.IsNullOrEmpty(message))
                return;

            var chatMessage = new
            {
                TableToken = tableToken,
                Text = message,
                Sender = "staff",
                Timestamp = DateTime.UtcNow,
                TableName = "Staff"
            };

            // === SỬA LỖI 1 (Dòng 97): Dùng GroupExcept ===
            // Gửi cho mọi người trong phòng, TRỪ NGƯỜI GỬI (chính là admin)
            await Clients.GroupExcept(tableToken, Context.ConnectionId)
                         .SendAsync("ReceiveMessage", chatMessage);
        }
    }
}