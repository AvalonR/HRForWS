using System.ComponentModel.DataAnnotations.Schema;
using HRAPI.Enums;

namespace HRAPI.Models;

public class Deduction
{
    public int Id { get; set; }

    public int PayrollRecordId { get; set; }

    [ForeignKey(nameof(PayrollRecordId))]
    public PayrollRecord PayrollRecord { get; set; } = null!;

    public DeductionType Type { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }
}
