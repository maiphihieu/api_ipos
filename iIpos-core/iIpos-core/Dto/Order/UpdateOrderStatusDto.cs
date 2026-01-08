using System.ComponentModel.DataAnnotations;

namespace iIpos_core.Dto.Order
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public string Status { get; set; } = null!;
    }
}
