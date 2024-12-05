using System.ComponentModel.DataAnnotations;
 using System.ComponentModel.DataAnnotations.Schema;
 using System.Text.Json.Serialization;
 using AirbnbMinimal.Enums;
 
 namespace AirbnbMinimal.Models;
 
 [Table("tb_USERS")]
 public class User : BaseModel
 {
     [Key]
     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
     [Column("ID")]
     public int Id { get; set; }

     [Required]
     [StringLength(30)]
     [Column("USERNAME", TypeName = "nvarchar(30)")]
     public string Username { get; set; } = string.Empty;

     [Required]
     [StringLength(30)]
     [Column("NAME", TypeName = "nvarchar(30)")]
     public string Name { get; set; } = string.Empty;
 
     [Required]
     [StringLength(30)]
     [Column("SURNAME", TypeName = "nvarchar(30)")]
     public string Surname { get; set; } = string.Empty;
 
     [EmailAddress]
     [StringLength(80)]
     [Column("EMAIL", TypeName = "nvarchar(80)")]
     public string Email { get; set; } = string.Empty;
     
     [Required]
     [StringLength(20)]
     [Column("PHONE", TypeName = "nvarchar(20)")]
     public string Phone { get; set; } = string.Empty;
 
     [Required]
     [Column("BIRTH_DATE")]
     public DateTime BirthDate { get; set; }
 
     [Required]
     [StringLength(120, MinimumLength = 8)]
     [Column("PASSWORD",TypeName = "nvarchar(240)")]
     public string Password { get; set; } = string.Empty;
 
     [Required]
     [Column("GENDER")]
     public Gender Gender { get; set; } = Gender.None;
     
     [Column("IS_BANNED")]
     public bool IsBanned { get; set; } = false;
     
     [StringLength(500)]
     [Column("PROFILE_IMAGE_URL", TypeName = "nvarchar(500)")]
     public string? ProfileImageUrl { get; set; } = string.Empty;
    
     [StringLength(1000)]
     [Column("BIOGRAPHY", TypeName = "nvarchar(1000)")]
     public string? Biography { get; set; } = string.Empty;
    
     [StringLength(500)]
     [Column("EXTRA_INFO", TypeName = "nvarchar(500)")]
     public string? ExtraInfo { get; set; } = string.Empty;
    
     [Required]
     [ForeignKey("Role")]
     public int RoleId { get; set; }
     
     public Role Role { get; set; } = null!;
     
     [JsonIgnore] 
     public ICollection<Listing> Listings { get; set; } = null!;
      
     [JsonIgnore] 
     public ICollection<Booking> Revisions { get; set; } = null!;
     
     [JsonIgnore] 
     public ICollection<Comment> Comments { get; set; } = null!;

 }