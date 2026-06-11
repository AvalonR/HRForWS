using System.ComponentModel.DataAnnotations;

namespace HRAPI.DTOs.Auth;

public class CurrentUserResponse
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public List<string> Roles { get; set; } = new List<string>();

    public int? EmployeeId { get; set; }
}
