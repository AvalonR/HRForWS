using System.ComponentModel.DataAnnotations;
using HRAPI.Enums;

namespace HRAPI.DTOs.PayrollRecords;

public class PayrollRecordCreateDto
{
    [Range(1, int.MaxValue)]
    public int EmployeeId { get; set; }

    public DateOnly PayPeriodStart { get; set; }
    public DateOnly PayPeriodEnd { get; set; }

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    public decimal BaseSalary { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal Overtime { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal Bonuses { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal DeductionsTotal { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal NetPay { get; set; }

    public DateOnly PayDate { get; set; }

    public PayrollStatus Status { get; set; }
}
