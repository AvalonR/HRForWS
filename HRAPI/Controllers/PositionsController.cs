using HRAPI.Data;
using HRAPI.DTOs.Positions;
using HRAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HRAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PositionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<IEnumerable<PositionReadDto>>> GetPositions()
    {
        var positions = await _context.Positions
            .AsNoTracking()
            .Include(p => p.Department)
            .Select(p => new PositionReadDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                MinSalary = p.MinSalary,
                MaxSalary = p.MaxSalary,
                DepartmentId = p.DepartmentId,
                DepartmentName = p.Department.Name,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();

        return Ok(positions);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<PositionReadDto>> GetPosition(int id)
    {
        var position = await _context.Positions
            .AsNoTracking()
            .Include(p => p.Department)
            .Where(p => p.Id == id)
            .Select(p => new PositionReadDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                MinSalary = p.MinSalary,
                MaxSalary = p.MaxSalary,
                DepartmentId = p.DepartmentId,
                DepartmentName = p.Department.Name,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (position == null)
        {
            return NotFound();
        }

        return Ok(position);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<PositionReadDto>> CreatePosition(PositionCreateDto createDto)
    {
        if (createDto.MinSalary.HasValue && createDto.MaxSalary.HasValue &&
            createDto.MinSalary > createDto.MaxSalary)
        {
            return BadRequest("Minimum salary cannot be greater than maximum salary.");
        }

        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == createDto.DepartmentId);

        if (department == null)
        {
            return BadRequest("Department does not exist.");
        }

        var duplicateExists = await _context.Positions
            .AnyAsync(p => p.Title == createDto.Title && p.DepartmentId == createDto.DepartmentId);

        if (duplicateExists)
        {
            return BadRequest("Position title already exists in this department.");
        }

        var position = new Position
        {
            Title = createDto.Title,
            Description = createDto.Description,
            MinSalary = createDto.MinSalary,
            MaxSalary = createDto.MaxSalary,
            DepartmentId = createDto.DepartmentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Positions.Add(position);
        await _context.SaveChangesAsync();

        var readDto = new PositionReadDto
        {
            Id = position.Id,
            Title = position.Title,
            Description = position.Description,
            MinSalary = position.MinSalary,
            MaxSalary = position.MaxSalary,
            DepartmentId = position.DepartmentId,
            DepartmentName = department.Name,
            IsActive = position.IsActive,
            CreatedAt = position.CreatedAt,
            UpdatedAt = position.UpdatedAt
        };

        return CreatedAtAction(nameof(GetPosition), new { id = position.Id }, readDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdatePosition(int id, PositionUpdateDto updateDto)
    {
        if (updateDto.MinSalary.HasValue && updateDto.MaxSalary.HasValue &&
            updateDto.MinSalary > updateDto.MaxSalary)
        {
            return BadRequest("Minimum salary cannot be greater than maximum salary.");
        }

        var position = await _context.Positions.FindAsync(id);

        if (position == null)
        {
            return NotFound();
        }

        var departmentExists = await _context.Departments
            .AnyAsync(d => d.Id == updateDto.DepartmentId);

        if (!departmentExists)
        {
            return BadRequest("Department does not exist.");
        }

        var duplicateExists = await _context.Positions
            .AnyAsync(p => p.Title == updateDto.Title &&
                           p.DepartmentId == updateDto.DepartmentId &&
                           p.Id != id);

        if (duplicateExists)
        {
            return BadRequest("Position title already exists in this department.");
        }

        position.Title = updateDto.Title;
        position.Description = updateDto.Description;
        position.MinSalary = updateDto.MinSalary;
        position.MaxSalary = updateDto.MaxSalary;
        position.DepartmentId = updateDto.DepartmentId;
        position.IsActive = updateDto.IsActive;
        position.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePosition(int id)
    {
        var position = await _context.Positions.FindAsync(id);

        if (position == null)
        {
            return NotFound();
        }

        var hasEmployees = await _context.Employees
            .AnyAsync(e => e.PositionId == id);

        if (hasEmployees)
        {
            return BadRequest("Cannot delete position because it has employees.");
        }

        _context.Positions.Remove(position);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}