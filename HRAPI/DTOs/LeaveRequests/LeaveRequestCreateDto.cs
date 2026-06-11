using System.ComponentModel.DataAnnotations;

namespace HRAPI.DTOs.LeaveRequests;

// DTO used when creating a leave request; the service sets new requests to Pending.
public class LeaveRequestCreateDto
{
    [Range(1, int.MaxValue)]
    public int EmployeeId { get; set; }

    [Range(1, int.MaxValue)]
    public int LeaveTypeId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Reason { get; set; }
}
