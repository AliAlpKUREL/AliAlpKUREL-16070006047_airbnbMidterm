using AirbnbMinimal.Enums;

namespace AirbnbMinimal.DTOs;

public class UpdateLocationDto
{
    public string? Name { get; set; }
    public LocationType? Type { get; set; } = LocationType.Ulke;
    public int? ParentLocationId { get; set; }
}