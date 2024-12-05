using AirbnbMinimal.Enums;

namespace AirbnbMinimal.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Biography { get; set; } = string.Empty;
    public string? ExtraInfo { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; } = string.Empty;
    public Gender Gender { get; set; } = Gender.None;
    public string RoleName { get; set; } = string.Empty;
    public bool IsBanned { get; set; } = false;

    public bool IsDeleted { get; set; } = false;
}