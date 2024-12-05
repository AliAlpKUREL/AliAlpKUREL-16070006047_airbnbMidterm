using AirbnbMinimal.Enums;

namespace AirbnbMinimal.DTOs;

public class UpdateUserDto
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Biography { get; set; } = string.Empty;
    public string? ExtraInfo { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; } = string.Empty;
}