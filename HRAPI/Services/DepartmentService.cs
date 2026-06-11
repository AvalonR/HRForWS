using HRAPI.Data;
using HRAPI.DTOs.Departments;
using HRAPI.Models;
using HRAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Services;

// Contains department rules such as parent validation, duplicate codes, and delete protection.
public class DepartmentService : IDepartmentService
{
    private readonly AppDbContext _context;

    public DepartmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DepartmentReadDto>> GetAllAsync()
    {
        return await _context.Departments
            .AsNoTracking()
            .Select(d => ToReadDto(d))
            .ToListAsync();
    }

    public async Task<DepartmentReadDto?> GetByIdAsync(int id)
    {
        return await _context.Departments
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => ToReadDto(d))
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<DepartmentReadDto>> CreateAsync(DepartmentCreateDto dto)
    {
        if (dto.ParentDepartmentId.HasValue)
        {
            var parentExists = await _context.Departments.AnyAsync(d => d.Id == dto.ParentDepartmentId.Value);
            if (!parentExists)
            {
                return ServiceResult<DepartmentReadDto>.Failure("Parent department does not exist.");
            }
        }

        var codeExists = await _context.Departments.AnyAsync(d => d.Code == dto.Code);
        if (codeExists)
        {
            return ServiceResult<DepartmentReadDto>.Failure("Department code already exists.");
        }

        var department = new Department
        {
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description,
            ParentDepartmentId = dto.ParentDepartmentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return ServiceResult<DepartmentReadDto>.Success(ToReadDto(department));
    }

    public async Task<ServiceResult> UpdateAsync(int id, DepartmentUpdateDto dto)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return ServiceResult.Missing();
        }

        if (dto.ParentDepartmentId == id)
        {
            return ServiceResult.Failure("A department cannot be its own parent.");
        }

        if (dto.ParentDepartmentId.HasValue)
        {
            var parentExists = await _context.Departments.AnyAsync(d => d.Id == dto.ParentDepartmentId.Value);
            if (!parentExists)
            {
                return ServiceResult.Failure("Parent department does not exist.");
            }
        }

        var codeExists = await _context.Departments.AnyAsync(d => d.Code == dto.Code && d.Id != id);
        if (codeExists)
        {
            return ServiceResult.Failure("Department code already exists.");
        }

        department.Name = dto.Name;
        department.Code = dto.Code;
        department.Description = dto.Description;
        department.ParentDepartmentId = dto.ParentDepartmentId;
        department.IsActive = dto.IsActive;
        department.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return ServiceResult.Missing();
        }

        var hasSubDepartments = await _context.Departments.AnyAsync(d => d.ParentDepartmentId == id);
        if (hasSubDepartments)
        {
            return ServiceResult.Failure("Cannot delete department because it has subdepartments.");
        }

        var hasEmployees = await _context.Employees.AnyAsync(e => e.DepartmentId == id);
        if (hasEmployees)
        {
            return ServiceResult.Failure("Cannot delete department because it has employees.");
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    private static DepartmentReadDto ToReadDto(Department department)
    {
        return new DepartmentReadDto
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
    }
}
