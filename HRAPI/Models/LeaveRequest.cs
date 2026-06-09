using System.ComponentModel.DataAnnotations.Schema;

namespace HRAPI.Models;

public class LeaveRequest
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;

    public int LeaveTypeId { get; set; }

    [ForeignKey(nameof(LeaveTypeId))]
    public LeaveType LeaveType { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Reason { get; set; }

    public DateTime DateRequested { get; set; }

    public int? ReviewedByEmployeeId { get; set; }

    [ForeignKey(nameof(ReviewedByEmployeeId))]
    public Employee? ReviewedByEmployee { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
