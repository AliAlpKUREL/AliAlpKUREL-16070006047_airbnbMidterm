using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirbnbMinimal.Models;

[Table("tb_COMMENTS")]
public class Comment : BaseModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [ForeignKey("User")]
    [Column("USER_ID")]
    public int UserId { get; set; }

    [StringLength(1000)]
    [Column("COMMENT_TEXT", TypeName = "nvarchar(1000)")]
    public string CommentText { get; set; } = string.Empty;

    [Required]
    [Column("RATING")]
    public int Rating { get; set; } 
    
    [ForeignKey("Listing")]
    [Column("LISTING_ID")]
    public int? ListingId { get; set; } 
    
    [ForeignKey("TargetUser")]
    [Column("TARGET_USER_ID")]
    public int? TargetUserId { get; set; } 

    public User User { get; set; } = null!;
    public Listing? Listing { get; set; } = null!;
    public User? TargetUser { get; set; } = null!;
}
