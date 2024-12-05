using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirbnbMinimal.Models;

public class BaseModel
{
    [Required]
    [Column("CREATED_DATE")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    [Required]
    [StringLength(20)]
    [Column("CREATED_VERSION", TypeName = "nvarchar(10)")]
    public string CreatedVersion { get; set; } = string.Empty;

    [Column("MODIFIED_DATE")]
    public DateTime? ModifiedDate { get; set; }

    [StringLength(20)]
    [Column("MODIFIED_VERSION", TypeName = "nvarchar(10)")]
    public string? ModifiedVersion { get; set; }
    
    [Column("IS_DELETED")]
    public bool IsDeleted { get; set; } = false;
}