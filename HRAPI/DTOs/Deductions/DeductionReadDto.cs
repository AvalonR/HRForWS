using HRAPI.Enums;

namespace HRAPI.DTOs.Deductions;

public class DeductionReadDto
{
    public int Id { get; set; }
    public int PayrollRecordId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DeductionType Type { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}
