using HRAPI.Data;
using HRAPI.DTOs.Departments;
using HRAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HRAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DepartmentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<IEnumerable<DepartmentReadDto>>> GetDepartments()
    {
        var departments = await _context.Departments
            .AsNoTracking()
            .Select(d => new DepartmentReadDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                Description = d.Description,
                ParentDepartmentId = d.ParentDepartmentId,
                IsActive = d.IsActive,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .ToListAsync();

        return Ok(departments);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<DepartmentReadDto>> GetDepartment(int id)
    {
        var department = await _context.Departments
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DepartmentReadDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                Description = d.Description,
                ParentDepartmentId = d.ParentDepartmentId,
                IsActive = d.IsActive,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (department == null)
        {
            return NotFound();
        }

        return Ok(department);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<DepartmentReadDto>> CreateDepartment(DepartmentCreateDto createDto)
    {
        if (createDto.ParentDepartmentId.HasValue)
        {
            var parentExists = await _context.Departments
                .AnyAsync(d => d.Id == createDto.ParentDepartmentId.Value);

            if (!parentExists)
            {
                return BadRequest("Parent department does not exist.");
            }
        }

        var codeExists = await _context.Departments
            .AnyAsync(d => d.Code == createDto.Code);

        if (codeExists)
        {
            return BadRequest("Department code already exists.");
        }

        var department = new Department
        {
            Name = createDto.Name,
            Code = createDto.Code,
            Description = createDto.Description,
            ParentDepartmentId = createDto.ParentDepartmentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        var readDto = new DepartmentReadDto
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code,
            Description = department.Description,
            ParentDepartmentId = department.ParentDepartmentId,
            IsActive = department.IsActive,
            CreatedAt = department.CreatedAt,
            UpdatedAt = department.UpdatedAt
        };

        return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, readDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdateDepartment(int id, DepartmentUpdateDto updateDto)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null)
        {
            return NotFound();
        }

        if (updateDto.ParentDepartmentId == id)
        {
            return BadRequest("A department cannot be its own parent.");
        }

        if (updateDto.ParentDepartmentId.HasValue)
        {
            var parentExists = await _context.Departments
                .AnyAsync(d => d.Id == updateDto.ParentDepartmentId.Value);

            if (!parentExists)
            {
                return BadRequest("Parent department does not exist.");
            }
        }

        var codeExists = await _context.Departments
            .AnyAsync(d => d.Code == updateDto.Code && d.Id != id);

        if (codeExists)
        {
            return BadRequest("Department code already exists.");
        }

        department.Name = updateDto.Name;
        department.Code = updateDto.Code;
        department.Description = updateDto.Description;
        department.ParentDepartmentId = updateDto.ParentDepartmentId;
        department.IsActive = updateDto.IsActive;
        department.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null)
        {
            return NotFound();
        }

        var hasSubDepartments = await _context.Departments
            .AnyAsync(d => d.ParentDepartmentId == id);

        if (hasSubDepartments)
        {
            return BadRequest("Cannot delete department because it has subdepartments.");
        }

        var hasEmployees = await _context.Employees
            .AnyAsync(e => e.DepartmentId == id);

        if (hasEmployees)
        {
            return BadRequest("Cannot delete department because it has employees.");
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
