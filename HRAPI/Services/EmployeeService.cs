using HRAPI.Data;
using HRAPI.DTOs.Employees;
using HRAPI.Models;
using HRAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;

    public EmployeeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeReadDto>> GetAllAsync()
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Manager)
            .Select(e => ToReadDto(e))
            .ToListAsync();
    }

    public async Task<EmployeeReadDto?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Manager)
            .Where(e => e.Id == id)
            .Select(e => ToReadDto(e))
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<EmployeeReadDto>> CreateAsync(EmployeeCreateDto dto)
    {
        var validationError = await ValidateEmployeeAsync(dto.EmployeeNumber, dto.Email, dto.HireDate, dto.TerminationDate, dto.DepartmentId, dto.PositionId, dto.ManagerId, null);
        if (validationError != null)
        {
            return ServiceResult<EmployeeReadDto>.Failure(validationError);
        }

        var department = await _context.Departments.FirstAsync(d => d.Id == dto.DepartmentId);
        var position = await _context.Positions.FirstAsync(p => p.Id == dto.PositionId);
        Employee? manager = null;

        if (dto.ManagerId.HasValue)
        {
            manager = await _context.Employees.FirstAsync(e => e.Id == dto.ManagerId.Value);
        }

        var employee = new Employee
        {
            EmployeeNumber = dto.EmployeeNumber,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            DateOfBirth = dto.DateOfBirth,
            HireDate = dto.HireDate,
            TerminationDate = dto.TerminationDate,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            PostalCode = dto.PostalCode,
            Country = dto.Country,
            DepartmentId = dto.DepartmentId,
            PositionId = dto.PositionId,
            ManagerId = dto.ManagerId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        employee.Department = department;
        employee.Position = position;
        employee.Manager = manager;

        return ServiceResult<EmployeeReadDto>.Success(ToReadDto(employee));
    }

    public async Task<ServiceResult> UpdateAsync(int id, EmployeeUpdateDto dto)
    {
        if (dto.ManagerId == id)
        {
            return ServiceResult.Failure("Employee cannot be their own manager.");
        }

        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return ServiceResult.Missing();
        }

        var validationError = await ValidateEmployeeAsync(dto.EmployeeNumber, dto.Email, dto.HireDate, dto.TerminationDate, dto.DepartmentId, dto.PositionId, dto.ManagerId, id);
        if (validationError != null)
        {
            return ServiceResult.Failure(validationError);
        }

        employee.EmployeeNumber = dto.EmployeeNumber;
        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;
        employee.Phone = dto.Phone;
        employee.DateOfBirth = dto.DateOfBirth;
        employee.HireDate = dto.HireDate;
        employee.TerminationDate = dto.TerminationDate;
        employee.Address = dto.Address;
        employee.City = dto.City;
        employee.State = dto.State;
        employee.PostalCode = dto.PostalCode;
        employee.Country = dto.Country;
        employee.DepartmentId = dto.DepartmentId;
        employee.PositionId = dto.PositionId;
        employee.ManagerId = dto.ManagerId;
        employee.IsActive = dto.IsActive;
        employee.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return ServiceResult.Missing();
        }

        var hasSubordinates = await _context.Employees.AnyAsync(e => e.ManagerId == id);
        if (hasSubordinates)
        {
            return ServiceResult.Failure("Cannot delete employee because they manage other employees.");
        }

        var hasLeaveRequests = await _context.LeaveRequests.AnyAsync(lr => lr.EmployeeId == id);
        if (hasLeaveRequests)
        {
            return ServiceResult.Failure("Cannot delete employee because they have leave requests.");
        }

        var hasAttendanceRecords = await _context.Attendances.AnyAsync(a => a.EmployeeId == id);
        if (hasAttendanceRecords)
        {
            return ServiceResult.Failure("Cannot delete employee because they have attendance records.");
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    private async Task<string?> ValidateEmployeeAsync(
        string employeeNumber,
        string email,
        DateOnly hireDate,
        DateOnly? terminationDate,
        int departmentId,
        int positionId,
        int? managerId,
        int? currentEmployeeId)
    {
        if (terminationDate.HasValue && terminationDate < hireDate)
        {
            return "Termination date cannot be before hire date.";
        }

        var employeeNumberExists = await _context.Employees.AnyAsync(e => e.EmployeeNumber == employeeNumber && (!currentEmployeeId.HasValue || e.Id != currentEmployeeId.Value));
        if (employeeNumberExists)
        {
            return "Employee number already exists.";
        }

        var emailExists = await _context.Employees.AnyAsync(e => e.Email == email && (!currentEmployeeId.HasValue || e.Id != currentEmployeeId.Value));
        if (emailExists)
        {
            return "Employee email already exists.";
        }

        var departmentExists = await _context.Departments.AnyAsync(d => d.Id == departmentId);
        if (!departmentExists)
        {
            return "Department does not exist.";
        }

        var position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == positionId);
        if (position == null)
        {
            return "Position does not exist.";
        }

        if (position.DepartmentId != departmentId)
        {
            return "Position does not belong to the selected department.";
        }

        if (managerId.HasValue)
        {
            var managerExists = await _context.Employees.AnyAsync(e => e.Id == managerId.Value);
            if (!managerExists)
            {
                return "Manager does not exist.";
            }
        }

        return null;
    }

    private static EmployeeReadDto ToReadDto(Employee employee)
    {
        return new EmployeeReadDto
        {
            Id = employee.Id,
            EmployeeNumber = employee.EmployeeNumber,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            DateOfBirth = employee.DateOfBirth,
            HireDate = employee.HireDate,
            TerminationDate = employee.TerminationDate,
            Address = employee.Address,
            City = employee.City,
            State = employee.State,
            PostalCode = employee.PostalCode,
            Country = employee.Country,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department.Name,
            PositionId = employee.PositionId,
            PositionTitle = employee.Position.Title,
            ManagerId = employee.ManagerId,
            ManagerName = employee.Manager == null ? null : employee.Manager.FirstName + " " + employee.Manager.LastName,
            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };
    }
}
