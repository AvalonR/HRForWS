using System.ComponentModel.DataAnnotations;

namespace HRAPI.DTOs.LeaveTypes;

// DTO used to update leave allowance and paid/unpaid settings.
public class LeaveTypeUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 365)]
    public int DaysAllowed { get; set; }

    public bool IsPaid { get; set; }
}