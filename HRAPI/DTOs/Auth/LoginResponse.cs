using System.ComponentModel.DataAnnotations;

namespace HRAPI.DTOs.Auth;

public class LoginResponse
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public List<string> Roles { get; set; } = new List<string>();
}
