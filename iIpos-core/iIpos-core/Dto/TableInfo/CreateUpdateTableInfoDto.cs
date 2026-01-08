using System.ComponentModel.DataAnnotations;

namespace iIpos_core.Dto.TableInfo
{
    public class CreateUpdateTableInfoDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        [Required]
        public int BranchId { get; set; }
    }
}
