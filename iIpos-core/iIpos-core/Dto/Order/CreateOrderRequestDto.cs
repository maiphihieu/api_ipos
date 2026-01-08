using System.ComponentModel.DataAnnotations;

namespace iIpos_core.Dto.Order
{
    public class CreateOrderRequestDto
    {
        [Required]
        public int BranchId { get; set; }
        [Required]
        public int TableInfoId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Order must have at least one item.")]
        public List<OrderItemRequestDto> OrderItems { get; set; } = new List<OrderItemRequestDto>();
    }

    public class OrderItemRequestDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
        public int Quantity { get; set; }
    }
}
