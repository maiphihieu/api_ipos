using System.ComponentModel.DataAnnotations;

namespace iIpos_core.Dto.Store
{
    public class CreateUpdateStoreDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? ImgStore { get; set; }

    }
}
