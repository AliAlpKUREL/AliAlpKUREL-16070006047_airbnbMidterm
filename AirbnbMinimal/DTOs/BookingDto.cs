namespace AirbnbMinimal.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public string ListingTitle { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NumberOfGuests { get; set; }
    public string Status { get; set; } = string.Empty;
}