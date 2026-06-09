using System.ComponentModel.DataAnnotations.Schema;
using HRAPI.Enums;

namespace HRAPI.Models;

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
