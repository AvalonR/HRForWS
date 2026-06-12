using HRAPI.Data;
using HRAPI.DTOs.PayrollRecords;
using HRAPI.Models;
using HRAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Services;

// Handles payroll validation for pay periods, amounts, and net pay consistency.
public class PayrollRecordService : IPayrollRecordService
{
    private readonly AppDbContext _context;

    public PayrollRecordService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PayrollRecordReadDto>> GetAllAsync()
    {
        return await _context.PayrollRecords
            .AsNoTracking()
            .Include(pr => pr.Employee)
            .Select(pr => ToReadDto(pr))
            .ToListAsync();
    }

    public async Task<PayrollRecordReadDto?> GetByIdAsync(int id)
    {
        return await _context.PayrollRecords
            .AsNoTracking()
            .Include(pr => pr.Employee)
            .Where(pr => pr.Id == id)
            .Select(pr => ToReadDto(pr))
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<PayrollRecordReadDto>> CreateAsync(PayrollRecordCreateDto dto)
    {
        var validationError = ValidatePayrollValues(
            dto.PayPeriodStart, dto.PayPeriodEnd,
            dto.BaseSalary, dto.Overtime, dto.Bonuses, dto.DeductionsTotal, dto.NetPay);

        if (validationError != null)
        {
            return ServiceResult<PayrollRecordReadDto>.Failure(validationError);
        }

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == dto.EmployeeId);
        if (employee == null)
        {
            return ServiceResult<PayrollRecordReadDto>.Failure("Employee does not exist.");
        }

        var payrollRecord = new PayrollRecord
        {
            EmployeeId = dto.EmployeeId,
            PayPeriodStart = dto.PayPeriodStart,
            PayPeriodEnd = dto.PayPeriodEnd,
            BaseSalary = dto.BaseSalary,
            Overtime = dto.Overtime,
            Bonuses = dto.Bonuses,
            DeductionsTotal = dto.DeductionsTotal,
            NetPay = dto.NetPay,
            PayDate = dto.PayDate,
            Status = dto.Status,
            CreatedAt = DateTime.UtcNow
        };

        _context.PayrollRecords.Add(payrollRecord);
        await _context.SaveChangesAsync();

        payrollRecord.Employee = employee;
        return ServiceResult<PayrollRecordReadDto>.Success(ToReadDto(payrollRecord));
    }

    public async Task<ServiceResult> UpdateAsync(int id, PayrollRecordUpdateDto dto)
    {
        var validationError = ValidatePayrollValues(
            dto.PayPeriodStart, dto.PayPeriodEnd,
            dto.BaseSalary, dto.Overtime, dto.Bonuses, dto.DeductionsTotal, dto.NetPay);

        if (validationError != null)
        {
            return ServiceResult.Failure(validationError);
        }

        var payrollRecord = await _context.PayrollRecords.FindAsync(id);
        if (payrollRecord == null)
        {
            return ServiceResult.Missing();
        }

        var employeeExists = await _context.Employees.AnyAsync(e => e.Id == dto.EmployeeId);
        if (!employeeExists)
        {
            return ServiceResult.Failure("Employee does not exist.");
        }

        payrollRecord.EmployeeId = dto.EmployeeId;
        payrollRecord.PayPeriodStart = dto.PayPeriodStart;
        payrollRecord.PayPeriodEnd = dto.PayPeriodEnd;
        payrollRecord.BaseSalary = dto.BaseSalary;
        payrollRecord.Overtime = dto.Overtime;
        payrollRecord.Bonuses = dto.Bonuses;
        payrollRecord.DeductionsTotal = dto.DeductionsTotal;
        payrollRecord.NetPay = dto.NetPay;
        payrollRecord.PayDate = dto.PayDate;
        payrollRecord.Status = dto.Status;
        payrollRecord.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var payrollRecord = await _context.PayrollRecords.FindAsync(id);
        if (payrollRecord == null)
        {
            return ServiceResult.Missing();
        }

        var hasDeductions = await _context.Deductions.AnyAsync(d => d.PayrollRecordId == id);
        if (hasDeductions)
        {
            return ServiceResult.Failure("Cannot delete payroll record because it has deductions.");
        }

        _context.PayrollRecords.Remove(payrollRecord);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    // Payroll validation keeps stored net pay consistent with salary, additions, and deductions.
    private static string? ValidatePayrollValues(
        DateOnly payPeriodStart, DateOnly payPeriodEnd,
        decimal baseSalary, decimal overtime, decimal bonuses,
        decimal deductionsTotal, decimal netPay)
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

    private static PayrollRecordReadDto ToReadDto(PayrollRecord payrollRecord)
    {
        return new PayrollRecordReadDto
        {
            Id = payrollRecord.Id,
            EmployeeId = payrollRecord.EmployeeId,
            EmployeeName = payrollRecord.Employee.FirstName + " " + payrollRecord.Employee.LastName,
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
    }
}
