using HRAPI.Data;
using HRAPI.DTOs.PayrollRecords;
using HRAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HRAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
// Manages payroll records and validates pay-period and payroll amount rules.
public class PayrollRecordsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PayrollRecordsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<IEnumerable<PayrollRecordReadDto>>> GetPayrollRecords()
    {
        var payrollRecords = await _context.PayrollRecords
            .AsNoTracking()
            .Include(pr => pr.Employee)
            .Select(pr => new PayrollRecordReadDto
            {
                Id = pr.Id,
                EmployeeId = pr.EmployeeId,
                EmployeeName = pr.Employee.FirstName + " " + pr.Employee.LastName,
                PayPeriodStart = pr.PayPeriodStart,
                PayPeriodEnd = pr.PayPeriodEnd,
                BaseSalary = pr.BaseSalary,
                Overtime = pr.Overtime,
                Bonuses = pr.Bonuses,
                DeductionsTotal = pr.DeductionsTotal,
                NetPay = pr.NetPay,
                PayDate = pr.PayDate,
                Status = pr.Status,
                CreatedAt = pr.CreatedAt,
                UpdatedAt = pr.UpdatedAt
            })
            .ToListAsync();

        return Ok(payrollRecords);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<PayrollRecordReadDto>> GetPayrollRecord(int id)
    {
        var payrollRecord = await _context.PayrollRecords
            .AsNoTracking()
            .Include(pr => pr.Employee)
            .Where(pr => pr.Id == id)
            .Select(pr => new PayrollRecordReadDto
            {
                Id = pr.Id,
                EmployeeId = pr.EmployeeId,
                EmployeeName = pr.Employee.FirstName + " " + pr.Employee.LastName,
                PayPeriodStart = pr.PayPeriodStart,
                PayPeriodEnd = pr.PayPeriodEnd,
                BaseSalary = pr.BaseSalary,
                Overtime = pr.Overtime,
                Bonuses = pr.Bonuses,
                DeductionsTotal = pr.DeductionsTotal,
                NetPay = pr.NetPay,
                PayDate = pr.PayDate,
                Status = pr.Status,
                CreatedAt = pr.CreatedAt,
                UpdatedAt = pr.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (payrollRecord == null)
        {
            return NotFound();
        }

        return Ok(payrollRecord);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<PayrollRecordReadDto>> CreatePayrollRecord(PayrollRecordCreateDto createDto)
    {
        var validationError = ValidatePayrollValues(
            createDto.PayPeriodStart,
            createDto.PayPeriodEnd,
            createDto.BaseSalary,
            createDto.Overtime,
            createDto.Bonuses,
            createDto.DeductionsTotal,
            createDto.NetPay);

        if (validationError != null)
        {
            return BadRequest(validationError);
        }

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == createDto.EmployeeId);

        if (employee == null)
        {
            return BadRequest("Employee does not exist.");
        }

        var payrollRecord = new PayrollRecord
        {
            EmployeeId = createDto.EmployeeId,
            PayPeriodStart = createDto.PayPeriodStart,
            PayPeriodEnd = createDto.PayPeriodEnd,
            BaseSalary = createDto.BaseSalary,
            Overtime = createDto.Overtime,
            Bonuses = createDto.Bonuses,
            DeductionsTotal = createDto.DeductionsTotal,
            NetPay = createDto.NetPay,
            PayDate = createDto.PayDate,
            Status = createDto.Status,
            CreatedAt = DateTime.UtcNow
        };

        _context.PayrollRecords.Add(payrollRecord);
        await _context.SaveChangesAsync();

        var readDto = new PayrollRecordReadDto
        {
            Id = payrollRecord.Id,
            EmployeeId = payrollRecord.EmployeeId,
            EmployeeName = employee.FirstName + " " + employee.LastName,
            PayPeriodStart = payrollRecord.PayPeriodStart,
            PayPeriodEnd = payrollRecord.PayPeriodEnd,
            BaseSalary = payrollRecord.BaseSalary,
            Overtime = payrollRecord.Overtime,
            Bonuses = payrollRecord.Bonuses,
            DeductionsTotal = payrollRecord.DeductionsTotal,
            NetPay = payrollRecord.NetPay,
            PayDate = payrollRecord.PayDate,
            Status = payrollRecord.Status,
            CreatedAt = payrollRecord.CreatedAt,
            UpdatedAt = payrollRecord.UpdatedAt
        };

        return CreatedAtAction(nameof(GetPayrollRecord), new { id = payrollRecord.Id }, readDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdatePayrollRecord(int id, PayrollRecordUpdateDto updateDto)
    {
        var validationError = ValidatePayrollValues(
            updateDto.PayPeriodStart,
            updateDto.PayPeriodEnd,
            updateDto.BaseSalary,
            updateDto.Overtime,
            updateDto.Bonuses,
            updateDto.DeductionsTotal,
            updateDto.NetPay);

        if (validationError != null)
        {
            return BadRequest(validationError);
        }

        var payrollRecord = await _context.PayrollRecords.FindAsync(id);

        if (payrollRecord == null)
        {
            return NotFound();
        }

        var employeeExists = await _context.Employees.AnyAsync(e => e.Id == updateDto.EmployeeId);

        if (!employeeExists)
        {
            return BadRequest("Employee does not exist.");
        }

        payrollRecord.EmployeeId = updateDto.EmployeeId;
        payrollRecord.PayPeriodStart = updateDto.PayPeriodStart;
        payrollRecord.PayPeriodEnd = updateDto.PayPeriodEnd;
        payrollRecord.BaseSalary = updateDto.BaseSalary;
        payrollRecord.Overtime = updateDto.Overtime;
        payrollRecord.Bonuses = updateDto.Bonuses;
        payrollRecord.DeductionsTotal = updateDto.DeductionsTotal;
        payrollRecord.NetPay = updateDto.NetPay;
        payrollRecord.PayDate = updateDto.PayDate;
        payrollRecord.Status = updateDto.Status;
        payrollRecord.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePayrollRecord(int id)
    {
        var payrollRecord = await _context.PayrollRecords.FindAsync(id);

        if (payrollRecord == null)
        {
            return NotFound();
        }

        var hasDeductions = await _context.Deductions.AnyAsync(d => d.PayrollRecordId == id);

        if (hasDeductions)
        {
            return BadRequest("Cannot delete payroll record because it has deductions.");
        }

        _context.PayrollRecords.Remove(payrollRecord);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static string? ValidatePayrollValues(
        DateOnly payPeriodStart,
        DateOnly payPeriodEnd,
        decimal baseSalary,
        decimal overtime,
        decimal bonuses,
        decimal deductionsTotal,
        decimal netPay)
    {
        if (payPeriodEnd < payPeriodStart)
        {
            return "Pay period end cannot be before pay period start.";
        }

        if (baseSalary <= 0)
        {
            return "Base salary must be greater than zero.";
        }

        if (overtime < 0 || bonuses < 0 || deductionsTotal < 0 || netPay < 0)
        {
            return "Payroll amounts cannot be negative.";
        }

        var expectedNetPay = baseSalary + overtime + bonuses - deductionsTotal;

        if (netPay != expectedNetPay)
        {
            return "Net pay must equal base salary plus overtime and bonuses minus deductions total.";
        }

        return null;
    }
}
