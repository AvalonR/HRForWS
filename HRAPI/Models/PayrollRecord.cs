using System.ComponentModel.DataAnnotations.Schema;
using HRAPI.Enums;

namespace HRAPI.Models;

// PayrollRecord stores payment values for one employee during one pay period.
public class PayrollRecord
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;

    public DateOnly PayPeriodStart { get; set; }
    public DateOnly PayPeriodEnd { get; set; }

    public decimal BaseSalary { get; set; }

    public decimal Overtime { get; set; } = 0;
    public decimal Bonuses { get; set; } = 0;
    public decimal DeductionsTotal { get; set; } = 0;

    public decimal NetPay { get; set; }

    public DateOnly PayDate { get; set; }

    public PayrollStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
