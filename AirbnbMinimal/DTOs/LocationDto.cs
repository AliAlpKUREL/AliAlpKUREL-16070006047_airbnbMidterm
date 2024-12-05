using AirbnbMinimal.Enums;

namespace AirbnbMinimal.DTOs;

public class LocationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public LocationType Type { get; set; } = LocationType.Ulke;
    public int? ParentLocationId { get; set; }
    public string? ParentLocationName { get; set; }
}