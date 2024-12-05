using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AirbnbMinimal.Enums;

namespace AirbnbMinimal.Models;

[Table("tb_BOOKINGS")]
public class Booking : BaseModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [Column("START_DATE")]
    public DateTime StartDate { get; set; }

    [Required]
    [Column("END_DATE")]
    public DateTime EndDate { get; set; }

    [Required]
    [Column("NUMBER_OF_GUESTS")]
    public int NumberOfGuests { get; set; }

    [Required]
    [Column("STATUS")]
    public BookingStatus Status { get; set; } = BookingStatus.Bekleniyor;

    [Required]
    [ForeignKey("Listing")]
    [Column("LISTING_ID")]
    public int ListingId { get; set; }
    
    [Required]
    [ForeignKey("User")]
    [Column("USER_ID")]
    public int UserId { get; set; }

    public Listing Listing { get; set; } = null!;
    public User User { get; set; } = null!;
}

