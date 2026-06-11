using HRAPI.Data;
using HRAPI.DTOs.Deductions;
using HRAPI.Models;
using HRAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Services;

// Handles deduction validation and ensures each deduction belongs to a payroll record.
public class DeductionService : IDeductionService
{
    private readonly AppDbContext _context;

    public DeductionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DeductionReadDto>> GetAllAsync()
    {
        return await _context.Deductions
            .AsNoTracking()
            .Include(d => d.PayrollRecord)
            .ThenInclude(pr => pr.Employee)
            .Select(d => new DeductionReadDto
            {
                Id = d.Id,
                PayrollRecordId = d.PayrollRecordId,
                EmployeeId = d.PayrollRecord.EmployeeId,
                EmployeeName = d.PayrollRecord.Employee.FirstName + " " + d.PayrollRecord.Employee.LastName,
                Type = d.Type,
                Amount = d.Amount,
                Description = d.Description
            })
            .ToListAsync();
    }

    public async Task<DeductionReadDto?> GetByIdAsync(int id)
    {
        return await _context.Deductions
            .AsNoTracking()
            .Include(d => d.PayrollRecord)
            .ThenInclude(pr => pr.Employee)
            .Where(d => d.Id == id)
            .Select(d => new DeductionReadDto
            {
                Id = d.Id,
                PayrollRecordId = d.PayrollRecordId,
                EmployeeId = d.PayrollRecord.EmployeeId,
                EmployeeName = d.PayrollRecord.Employee.FirstName + " " + d.PayrollRecord.Employee.LastName,
                Type = d.Type,
                Amount = d.Amount,
                Description = d.Description
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<DeductionReadDto>> CreateAsync(DeductionCreateDto dto)
    {
        if (dto.Amount <= 0)
            return ServiceResult<DeductionReadDto>.Failure("Deduction amount must be greater than zero.");

        var payrollRecord = await _context.PayrollRecords
            .Include(pr => pr.Employee)
            .FirstOrDefaultAsync(pr => pr.Id == dto.PayrollRecordId);

        if (payrollRecord == null)
            return ServiceResult<DeductionReadDto>.Failure("Payroll record does not exist.");

        var deduction = new Deduction
        {
            PayrollRecordId = dto.PayrollRecordId,
            Type = dto.Type,
            Amount = dto.Amount,
            Description = dto.Description
        };

        _context.Deductions.Add(deduction);
        await _context.SaveChangesAsync();

        return ServiceResult<DeductionReadDto>.Success(new DeductionReadDto
        {
            Id = deduction.Id,
            PayrollRecordId = deduction.PayrollRecordId,
            EmployeeId = payrollRecord.EmployeeId,
            EmployeeName = payrollRecord.Employee.FirstName + " " + payrollRecord.Employee.LastName,
            Type = deduction.Type,
            Amount = deduction.Amount,
            Description = deduction.Description
        });
    }

    public async Task<ServiceResult> UpdateAsync(int id, DeductionUpdateDto dto)
    {
        if (dto.Amount <= 0)
            return ServiceResult.Failure("Deduction amount must be greater than zero.");

        var deduction = await _context.Deductions.FindAsync(id);
        if (deduction == null)
            return ServiceResult.Missing();

        var payrollRecordExists = await _context.PayrollRecords.AnyAsync(pr => pr.Id == dto.PayrollRecordId);
        if (!payrollRecordExists)
            return ServiceResult.Failure("Payroll record does not exist.");

        deduction.PayrollRecordId = dto.PayrollRecordId;
        deduction.Type = dto.Type;
        deduction.Amount = dto.Amount;
        deduction.Description = dto.Description;

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var deduction = await _context.Deductions.FindAsync(id);
        if (deduction == null)
            return ServiceResult.Missing();

        _context.Deductions.Remove(deduction);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }
}
