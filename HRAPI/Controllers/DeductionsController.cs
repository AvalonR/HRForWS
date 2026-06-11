using HRAPI.Data;
using HRAPI.DTOs.Deductions;
using HRAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HRAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeductionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DeductionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<IEnumerable<DeductionReadDto>>> GetDeductions()
    {
        var deductions = await _context.Deductions
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

        return Ok(deductions);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<DeductionReadDto>> GetDeduction(int id)
    {
        var deduction = await _context.Deductions
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

        if (deduction == null)
        {
            return NotFound();
        }

        return Ok(deduction);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<DeductionReadDto>> CreateDeduction(DeductionCreateDto createDto)
    {
        if (createDto.Amount <= 0)
        {
            return BadRequest("Deduction amount must be greater than zero.");
        }

        var payrollRecord = await _context.PayrollRecords
            .Include(pr => pr.Employee)
            .FirstOrDefaultAsync(pr => pr.Id == createDto.PayrollRecordId);

        if (payrollRecord == null)
        {
            return BadRequest("Payroll record does not exist.");
        }

        var deduction = new Deduction
        {
            PayrollRecordId = createDto.PayrollRecordId,
            Type = createDto.Type,
            Amount = createDto.Amount,
            Description = createDto.Description
        };

        _context.Deductions.Add(deduction);
        await _context.SaveChangesAsync();

        var readDto = new DeductionReadDto
        {
            Id = deduction.Id,
            PayrollRecordId = deduction.PayrollRecordId,
            EmployeeId = payrollRecord.EmployeeId,
            EmployeeName = payrollRecord.Employee.FirstName + " " + payrollRecord.Employee.LastName,
            Type = deduction.Type,
            Amount = deduction.Amount,
            Description = deduction.Description
        };

        return CreatedAtAction(nameof(GetDeduction), new { id = deduction.Id }, readDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdateDeduction(int id, DeductionUpdateDto updateDto)
    {
        if (updateDto.Amount <= 0)
        {
            return BadRequest("Deduction amount must be greater than zero.");
        }

        var deduction = await _context.Deductions.FindAsync(id);

        if (deduction == null)
        {
            return NotFound();
        }

        var payrollRecordExists = await _context.PayrollRecords.AnyAsync(pr => pr.Id == updateDto.PayrollRecordId);

        if (!payrollRecordExists)
        {
            return BadRequest("Payroll record does not exist.");
        }

        deduction.PayrollRecordId = updateDto.PayrollRecordId;
        deduction.Type = updateDto.Type;
        deduction.Amount = updateDto.Amount;
        deduction.Description = updateDto.Description;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDeduction(int id)
    {
        var deduction = await _context.Deductions.FindAsync(id);

        if (deduction == null)
        {
            return NotFound();
        }

        _context.Deductions.Remove(deduction);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
