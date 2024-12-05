namespace AirbnbMinimal.DTOs;

public class UpdateListingDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? PricePerNight { get; set; }
    public int? MaxGuests { get; set; }
    public int? RoomCount { get; set; }
    public int? BedCount { get; set; }
    public int? BathroomCount { get; set; }
    public string? FullAddress { get; set; }
    public string? LocationMap { get; set; }
    public DateTime? StartedDate { get; set; }
    public DateTime? EndDate { get; set; }
}