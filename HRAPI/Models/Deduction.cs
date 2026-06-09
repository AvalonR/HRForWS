using System.ComponentModel.DataAnnotations.Schema;

namespace HRAPI.Models;

public class Deduction
{
    public int Id { get; set; }

    public int PayrollRecordId { get; set; }

    [ForeignKey(nameof(PayrollRecordId))]
    public PayrollRecord PayrollRecord { get; set; } = null!;

    public string Type { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string? Description { get; set; }
}
