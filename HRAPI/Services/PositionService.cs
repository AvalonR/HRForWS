using HRAPI.Data;
using HRAPI.DTOs.Positions;
using HRAPI.Models;
using HRAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Services;

// Contains position validation, duplicate checks, and mapping between Position entities and DTOs.
public class PositionService : IPositionService
{
    private readonly AppDbContext _context;

    public PositionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PositionReadDto>> GetAllAsync()
    {
        return await _context.Positions
            .AsNoTracking()
            .Include(p => p.Department)
            .Select(p => ToReadDto(p))
            .ToListAsync();
    }

    public async Task<PositionReadDto?> GetByIdAsync(int id)
    {
        return await _context.Positions
            .AsNoTracking()
            .Include(p => p.Department)
            .Where(p => p.Id == id)
            .Select(p => ToReadDto(p))
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<PositionReadDto>> CreateAsync(PositionCreateDto dto)
    {
        if (dto.MinSalary.HasValue && dto.MaxSalary.HasValue && dto.MinSalary > dto.MaxSalary)
        {
            return ServiceResult<PositionReadDto>.Failure("Minimum salary cannot be greater than maximum salary.");
        }

        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == dto.DepartmentId);
        if (department == null)
        {
            return ServiceResult<PositionReadDto>.Failure("Department does not exist.");
        }

        // A department should not contain two positions with the same title.
        var duplicateExists = await _context.Positions.AnyAsync(p => p.Title == dto.Title && p.DepartmentId == dto.DepartmentId);
        if (duplicateExists)
        {
            return ServiceResult<PositionReadDto>.Failure("Position title already exists in this department.");
        }

        var position = new Position
        {
            Title = dto.Title,
            Description = dto.Description,
            MinSalary = dto.MinSalary,
            MaxSalary = dto.MaxSalary,
            DepartmentId = dto.DepartmentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Positions.Add(position);
        await _context.SaveChangesAsync();
        position.Department = department;

        return ServiceResult<PositionReadDto>.Success(ToReadDto(position));
    }

    public async Task<ServiceResult> UpdateAsync(int id, PositionUpdateDto dto)
    {
        if (dto.MinSalary.HasValue && dto.MaxSalary.HasValue && dto.MinSalary > dto.MaxSalary)
        {
            return ServiceResult.Failure("Minimum salary cannot be greater than maximum salary.");
        }

        var position = await _context.Positions.FindAsync(id);
        if (position == null)
        {
            return ServiceResult.Missing();
        }

        var departmentExists = await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId);
        if (!departmentExists)
        {
            return ServiceResult.Failure("Department does not exist.");
        }

        var duplicateExists = await _context.Positions.AnyAsync(p => p.Title == dto.Title && p.DepartmentId == dto.DepartmentId && p.Id != id);
        if (duplicateExists)
        {
            return ServiceResult.Failure("Position title already exists in this department.");
        }

        position.Title = dto.Title;
        position.Description = dto.Description;
        position.MinSalary = dto.MinSalary;
        position.MaxSalary = dto.MaxSalary;
        position.DepartmentId = dto.DepartmentId;
        position.IsActive = dto.IsActive;
        position.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var position = await _context.Positions.FindAsync(id);
        if (position == null)
        {
            return ServiceResult.Missing();
        }

        var hasEmployees = await _context.Employees.AnyAsync(e => e.PositionId == id);
        if (hasEmployees)
        {
            return ServiceResult.Failure("Cannot delete position because it has employees.");
        }

        _context.Positions.Remove(position);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    private static PositionReadDto ToReadDto(Position position)
    {
        return new PositionReadDto
        {
            Id = position.Id,
            Title = position.Title,
            Description = position.Description,
            MinSalary = position.MinSalary,
            MaxSalary = position.MaxSalary,
            DepartmentId = position.DepartmentId,
            DepartmentName = position.Department.Name,
            IsActive = position.IsActive,
            CreatedAt = position.CreatedAt,
            UpdatedAt = position.UpdatedAt
        };
    }
}
