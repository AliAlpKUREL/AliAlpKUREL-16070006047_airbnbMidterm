using System.ComponentModel.DataAnnotations;

namespace AirbnbMinimal.DTOs;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "The old password is required.")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "The password must be at least 8 characters long.")]
    public string NewPassword { get; set; } = string.Empty; 
}