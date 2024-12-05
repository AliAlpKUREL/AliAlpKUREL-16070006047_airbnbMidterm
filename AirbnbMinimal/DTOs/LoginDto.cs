using System.ComponentModel.DataAnnotations;

namespace AirbnbMinimal.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Kullanıcı adı veya e-posta gereklidir.")]
    public string UsernameOrMail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre gereklidir.")]
    public string Password { get; set; } = string.Empty;
}