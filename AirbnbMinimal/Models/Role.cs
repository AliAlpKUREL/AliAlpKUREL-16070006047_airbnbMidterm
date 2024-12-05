using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AirbnbMinimal.Models;

[Table("tb_ROLES")]
public class Role : BaseModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID")]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    [Column("NAME", TypeName = "nvarchar(50)")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(250)]
    [Column("DESCRIPTION", TypeName = "nvarchar(250)")]
    public string? Description { get; set; }

    [JsonIgnore] 
    public ICollection<User>? Users { get; set; } = null!;
}