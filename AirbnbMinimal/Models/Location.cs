using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AirbnbMinimal.Enums;

namespace AirbnbMinimal.Models;

[Table("tb_LOCATION")]
public class Location : BaseModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Column("NAME", TypeName = "nvarchar(100)")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Column("TYPE", TypeName = "nvarchar(50)")]
    public LocationType Type { get; set; } = LocationType.Ulke;

    public int? ParentLocationId { get; set; }

    [ForeignKey("ParentLocationId")]
    public Location? ParentLocation { get; set; }
}