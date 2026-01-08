using iIpos_core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace iIpos_core.Data
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string PasswordHash { get; set; } = null!;
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        [Column(TypeName = "nvarchar(24)")]
        public Role Role { get; set; } = Role.Staff; 
    }
}
