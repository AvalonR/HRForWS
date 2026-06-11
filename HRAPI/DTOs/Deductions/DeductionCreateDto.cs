using System.ComponentModel.DataAnnotations;
using HRAPI.Enums;

namespace HRAPI.DTOs.Deductions;

// DTO used to create a deduction connected to an existing payroll record.
public class DeductionCreateDto
{
    [Range(1, int.MaxValue)]
    public int PayrollRecordId { get; set; }

    public DeductionType Type { get; set; }

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    public decimal Amount { get; set; }

    public string? Description { get; set; }
}
