using System.ComponentModel.DataAnnotations.Schema;
using HRAPI.Enums;

namespace HRAPI.Models;

// Leave request connects an employee to a leave type for a specific date range.
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

    public LeaveRequestStatus Status { get; set; }

    public string? Reason { get; set; }

    public DateTime DateRequested { get; set; }

    // Optional reviewer is filled when HR reviews the request.
    public int? ReviewedByEmployeeId { get; set; }

    [ForeignKey(nameof(ReviewedByEmployeeId))]
    public Employee? ReviewedByEmployee { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
