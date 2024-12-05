namespace AirbnbMinimal.DTOs;

public class AddBookingDto
{
    public int ListingId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NumberOfGuests { get; set; }
}