namespace AirbnbMinimal.DTOs;

public class AddListingDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int MaxGuests { get; set; }
    public int RoomCount { get; set; }
    public int BedCount { get; set; }
    public int BathroomCount { get; set; }
    public string FullAddress { get; set; } = string.Empty;
    public string LocationMap { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public DateTime StartedDate { get; set; }
    public DateTime EndDate { get; set; }
}