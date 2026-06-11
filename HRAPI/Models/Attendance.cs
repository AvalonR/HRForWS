using System.ComponentModel.DataAnnotations.Schema;
using HRAPI.Enums;

namespace HRAPI.Models;

// Attendance stores one employee's status and optional clock times for a date.
public class Attendance
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;

    public DateOnly Date { get; set; }

    public TimeOnly? CheckIn { get; set; }
    public TimeOnly? CheckOut { get; set; }

    public AttendanceStatus Status { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
