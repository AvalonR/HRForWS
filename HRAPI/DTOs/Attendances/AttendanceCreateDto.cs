using System.ComponentModel.DataAnnotations;
using HRAPI.Enums;

namespace HRAPI.DTOs.Attendances;

public class AttendanceCreateDto
{
    [Range(1, int.MaxValue)]
    public int EmployeeId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly? CheckIn { get; set; }

    public TimeOnly? CheckOut { get; set; }

    public AttendanceStatus Status { get; set; }

    public string? Notes { get; set; }
}
