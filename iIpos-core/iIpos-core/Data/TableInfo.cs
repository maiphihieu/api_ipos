using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iIpos_core.Data
{
    [Table("Tables")]
    public class TableInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        public int BranchId { get; set; }
        public string Token { get; set; } = null!;
        public Branch? Branch { get; set; }
    }
}
