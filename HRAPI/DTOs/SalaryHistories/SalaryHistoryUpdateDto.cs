using System.ComponentModel.DataAnnotations;

namespace HRAPI.DTOs.SalaryHistories;

public class SalaryHistoryUpdateDto
{
    [Range(1, int.MaxValue)]
    public int EmployeeId { get; set; }

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    public decimal Amount { get; set; }

    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }

    public string? ChangeReason { get; set; }
}
