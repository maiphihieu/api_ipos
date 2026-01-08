using System.ComponentModel.DataAnnotations;

namespace iIpos_core.Dto.Branch
{
    public class CreateUpdateBranchDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Address { get; set; }

        [Required]
        public int StoreId { get; set; }
    }
}
