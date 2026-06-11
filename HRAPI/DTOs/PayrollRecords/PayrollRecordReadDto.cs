using HRAPI.Enums;

namespace HRAPI.DTOs.PayrollRecords;

// DTO returned for payroll records with employee name and calculated/stored payroll values.
public class PayrollRecordReadDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateOnly PayPeriodStart { get; set; }
    public DateOnly PayPeriodEnd { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal Overtime { get; set; }
    public decimal Bonuses { get; set; }
    public decimal DeductionsTotal { get; set; }
    public decimal NetPay { get; set; }
    public DateOnly PayDate { get; set; }
    public PayrollStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
