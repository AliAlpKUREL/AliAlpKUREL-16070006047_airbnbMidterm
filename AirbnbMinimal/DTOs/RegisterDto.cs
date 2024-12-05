using System.ComponentModel.DataAnnotations;
using AirbnbMinimal.Enums;

namespace AirbnbMinimal.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "The namespace is required.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "The surname field is required.")]
    public string  Surname { get; set; } = string.Empty;

    [Required(ErrorMessage = "The email field is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "The phone field is required.")]
    [Phone(ErrorMessage = "Please enter a valid phone number.")]
    public string  Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be at least 3 characters long.")]
    public string  Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "The password must be at least 8 characters long.")]
    public string  Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required.")]
    public Gender Gender { get; set; } = Gender.None;

    [Required(ErrorMessage = "Date of birth is required.")]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Role required.")]
    public int RoleId { get; set; }
}