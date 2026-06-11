using HRAPI.Data;
using HRAPI.DTOs.SalaryHistories;
using HRAPI.Models;
using HRAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Services;

public class SalaryHistoryService : ISalaryHistoryService
{
    private readonly AppDbContext _context;

    public SalaryHistoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SalaryHistoryReadDto>> GetAllAsync()
    {
        return await _context.SalaryHistories
            .AsNoTracking()
            .Include(sh => sh.Employee)
            .Select(sh => new SalaryHistoryReadDto
            {
                Id = sh.Id,
                EmployeeId = sh.EmployeeId,
                EmployeeName = sh.Employee.FirstName + " " + sh.Employee.LastName,
                Amount = sh.Amount,
                EffectiveFrom = sh.EffectiveFrom,
                EffectiveTo = sh.EffectiveTo,
                ChangeReason = sh.ChangeReason,
                CreatedAt = sh.CreatedAt,
                UpdatedAt = sh.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<SalaryHistoryReadDto?> GetByIdAsync(int id)
    {
        return await _context.SalaryHistories
            .AsNoTracking()
            .Include(sh => sh.Employee)
            .Where(sh => sh.Id == id)
            .Select(sh => new SalaryHistoryReadDto
            {
                Id = sh.Id,
                EmployeeId = sh.EmployeeId,
                EmployeeName = sh.Employee.FirstName + " " + sh.Employee.LastName,
                Amount = sh.Amount,
                EffectiveFrom = sh.EffectiveFrom,
                EffectiveTo = sh.EffectiveTo,
                ChangeReason = sh.ChangeReason,
                CreatedAt = sh.CreatedAt,
                UpdatedAt = sh.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<SalaryHistoryReadDto>> CreateAsync(SalaryHistoryCreateDto dto)
    {
        if (dto.Amount <= 0)
            return ServiceResult<SalaryHistoryReadDto>.Failure("Salary amount must be greater than zero.");

        if (dto.EffectiveTo.HasValue && dto.EffectiveTo < dto.EffectiveFrom)
            return ServiceResult<SalaryHistoryReadDto>.Failure("Effective to date cannot be before effective from date.");

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == dto.EmployeeId);
        if (employee == null)
            return ServiceResult<SalaryHistoryReadDto>.Failure("Employee does not exist.");

        var salaryHistory = new SalaryHistory
        {
            EmployeeId = dto.EmployeeId,
            Amount = dto.Amount,
            EffectiveFrom = dto.EffectiveFrom,
            EffectiveTo = dto.EffectiveTo,
            ChangeReason = dto.ChangeReason,
            CreatedAt = DateTime.UtcNow
        };

        _context.SalaryHistories.Add(salaryHistory);
        await _context.SaveChangesAsync();

        return ServiceResult<SalaryHistoryReadDto>.Success(new SalaryHistoryReadDto
        {
            Id = salaryHistory.Id,
            EmployeeId = salaryHistory.EmployeeId,
            EmployeeName = employee.FirstName + " " + employee.LastName,
            Amount = salaryHistory.Amount,
            EffectiveFrom = salaryHistory.EffectiveFrom,
            EffectiveTo = salaryHistory.EffectiveTo,
            ChangeReason = salaryHistory.ChangeReason,
            CreatedAt = salaryHistory.CreatedAt,
            UpdatedAt = salaryHistory.UpdatedAt
        });
    }

    public async Task<ServiceResult> UpdateAsync(int id, SalaryHistoryUpdateDto dto)
    {
        if (dto.Amount <= 0)
            return ServiceResult.Failure("Salary amount must be greater than zero.");

        if (dto.EffectiveTo.HasValue && dto.EffectiveTo < dto.EffectiveFrom)
            return ServiceResult.Failure("Effective to date cannot be before effective from date.");

        var salaryHistory = await _context.SalaryHistories.FindAsync(id);
        if (salaryHistory == null)
            return ServiceResult.Missing();

        var employeeExists = await _context.Employees.AnyAsync(e => e.Id == dto.EmployeeId);
        if (!employeeExists)
            return ServiceResult.Failure("Employee does not exist.");

        salaryHistory.EmployeeId = dto.EmployeeId;
        salaryHistory.Amount = dto.Amount;
        salaryHistory.EffectiveFrom = dto.EffectiveFrom;
        salaryHistory.EffectiveTo = dto.EffectiveTo;
        salaryHistory.ChangeReason = dto.ChangeReason;
        salaryHistory.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var salaryHistory = await _context.SalaryHistories.FindAsync(id);
        if (salaryHistory == null)
            return ServiceResult.Missing();

        _context.SalaryHistories.Remove(salaryHistory);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }
}
