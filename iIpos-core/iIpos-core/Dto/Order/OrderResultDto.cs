namespace iIpos_core.Dto.Order
{
    public class OrderResultDto
    {
        public int Id { get; set; }
        public DateTime OrderTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public string? TableName { get; set; } = null!;
        public int BranchId { get; set; }
        public List<OrderItemResultDto> Items { get; set; } = new List<OrderItemResultDto>();
    }

    public class OrderItemResultDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
