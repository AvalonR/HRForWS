using HRAPI.Enums;

namespace HRAPI.DTOs.LeaveRequests;

// DTO returned for leave requests with employee, leave type, and reviewer display names.
public class LeaveRequestReadDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public LeaveRequestStatus Status { get; set; }
    public string? Reason { get; set; }
    public DateTime DateRequested { get; set; }
    public int? ReviewedByEmployeeId { get; set; }
    public string? ReviewedByEmployeeName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
