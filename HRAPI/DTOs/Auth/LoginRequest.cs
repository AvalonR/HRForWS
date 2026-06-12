using System.ComponentModel.DataAnnotations;

namespace HRAPI.DTOs.Auth;

// DTO sent by the client when requesting a JWT token.
public class LoginRequest
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
