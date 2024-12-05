using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AirbnbMinimal.Models;

[Table("tb_LISTINGS")]
public class Listing : BaseModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID")]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    [Column("TITLE", TypeName = "nvarchar(100)")]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(1000)]
    [Column("DESCRIPTION", TypeName = "nvarchar(1000)")]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Column("PRICE_PER_NIGHT", TypeName = "decimal(18, 2)")]
    public decimal PricePerNight { get; set; }
    
    [Required]
    [Column("MAX_GUESTS")]
    public int MaxGuests { get; set; }
    
    [Required]
    [Column("ROOM_COUNT")]
    public int RoomCount { get; set; }
    
    [Required]
    [Column("BED_COUNT")]
    public int BedCount { get; set; }
    
    [Required]
    [Column("BATHROOM_COUNT")]
    public int BathroomCount { get; set; }

    [StringLength(500)]
    [Column("FULL_ADDRESS", TypeName = "nvarchar(500)")]
    public string FullAddress { get; set; } = string.Empty;
    
    [StringLength(500)]
    [Column("LOCATION_MAP", TypeName = "nvarchar(500)")]
    public string LocationMap { get; set; } = string.Empty;
    
    [Required]
    [Column("STARTED_DATE")]
    public DateTime StartedDate { get; set; } = DateTime.Today;
    
    [Required]
    [Column("END_DATE")]
    public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);
    
    [Required]
    [Column("USER_ID")]
    public int UserId { get; set; }
    
    public User User { get; set; } = null!;
    
    [Required]
    [ForeignKey("Location")]
    [Column("LOCATION_ID")]
    public int LocationId { get; set; }

    public Location Location { get; set; } = null!;

    [JsonIgnore]
    public ICollection<Booking> Bookings { get; set; } = null!;
    
    [JsonIgnore]
    public ICollection<Comment> Comments { get; set; } = null!;

}