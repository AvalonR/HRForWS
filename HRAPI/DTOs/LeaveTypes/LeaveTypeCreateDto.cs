using System.ComponentModel.DataAnnotations;

namespace HRAPI.DTOs.LeaveTypes;

public class LeaveTypeCreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 365)]
    public int DaysAllowed { get; set; }

    public bool IsPaid { get; set; }
}