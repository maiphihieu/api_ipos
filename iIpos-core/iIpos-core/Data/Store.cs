using System.ComponentModel.DataAnnotations;

namespace iIpos_core.Data
{
    public class Store
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!; 

        [MaxLength(500)]
       public string? ImgStore { get; set; }
     
    }
}
