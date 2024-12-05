using AirbnbMinimal.Enums;

namespace AirbnbMinimal.DTOs;

public class AddLocationDto
{
    public string Name { get; set; } = string.Empty;
    public LocationType Type { get; set; } = LocationType.Ulke;
    public int? ParentLocationId { get; set; }
}