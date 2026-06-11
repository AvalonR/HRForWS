using System.ComponentModel.DataAnnotations;

namespace HRAPI.DTOs.LeaveTypes;

// DTO used to create a leave type such as annual, sick, or personal leave.
public class LeaveTypeCreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 365)]
    public int DaysAllowed { get; set; }

    public bool IsPaid { get; set; }
}
