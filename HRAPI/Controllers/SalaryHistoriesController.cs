using HRAPI.Data;
using HRAPI.DTOs.SalaryHistories;
using HRAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HRAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalaryHistoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public SalaryHistoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<IEnumerable<SalaryHistoryReadDto>>> GetSalaryHistories()
    {
        var salaryHistories = await _context.SalaryHistories
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

        return Ok(salaryHistories);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<SalaryHistoryReadDto>> GetSalaryHistory(int id)
    {
        var salaryHistory = await _context.SalaryHistories
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

        if (salaryHistory == null)
        {
            return NotFound();
        }

        return Ok(salaryHistory);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<SalaryHistoryReadDto>> CreateSalaryHistory(SalaryHistoryCreateDto createDto)
    {
        if (createDto.Amount <= 0)
        {
            return BadRequest("Salary amount must be greater than zero.");
        }

        if (createDto.EffectiveTo.HasValue && createDto.EffectiveTo < createDto.EffectiveFrom)
        {
            return BadRequest("Effective to date cannot be before effective from date.");
        }

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == createDto.EmployeeId);

        if (employee == null)
        {
            return BadRequest("Employee does not exist.");
        }

        var salaryHistory = new SalaryHistory
        {
            EmployeeId = createDto.EmployeeId,
            Amount = createDto.Amount,
            EffectiveFrom = createDto.EffectiveFrom,
            EffectiveTo = createDto.EffectiveTo,
            ChangeReason = createDto.ChangeReason,
            CreatedAt = DateTime.UtcNow
        };

        _context.SalaryHistories.Add(salaryHistory);
        await _context.SaveChangesAsync();

        var readDto = new SalaryHistoryReadDto
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
        };

        return CreatedAtAction(nameof(GetSalaryHistory), new { id = salaryHistory.Id }, readDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdateSalaryHistory(int id, SalaryHistoryUpdateDto updateDto)
    {
        if (updateDto.Amount <= 0)
        {
            return BadRequest("Salary amount must be greater than zero.");
        }

        if (updateDto.EffectiveTo.HasValue && updateDto.EffectiveTo < updateDto.EffectiveFrom)
        {
            return BadRequest("Effective to date cannot be before effective from date.");
        }

        var salaryHistory = await _context.SalaryHistories.FindAsync(id);

        if (salaryHistory == null)
        {
            return NotFound();
        }

        var employeeExists = await _context.Employees.AnyAsync(e => e.Id == updateDto.EmployeeId);

        if (!employeeExists)
        {
            return BadRequest("Employee does not exist.");
        }

        salaryHistory.EmployeeId = updateDto.EmployeeId;
        salaryHistory.Amount = updateDto.Amount;
        salaryHistory.EffectiveFrom = updateDto.EffectiveFrom;
        salaryHistory.EffectiveTo = updateDto.EffectiveTo;
        salaryHistory.ChangeReason = updateDto.ChangeReason;
        salaryHistory.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteSalaryHistory(int id)
    {
        var salaryHistory = await _context.SalaryHistories.FindAsync(id);

        if (salaryHistory == null)
        {
            return NotFound();
        }

        _context.SalaryHistories.Remove(salaryHistory);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
