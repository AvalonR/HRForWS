using System.ComponentModel.DataAnnotations.Schema;

namespace HRAPI.Models;

// SalaryHistory records salary changes over time instead of overwriting old values.
public class SalaryHistory
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }

    public string? ChangeReason { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
