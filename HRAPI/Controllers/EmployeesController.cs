using HRAPI.Data;
using HRAPI.DTOs.Employees;
using HRAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeReadDto>>> GetEmployees()
    {
        var employees = await _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Manager)
            .Select(e => new EmployeeReadDto
            {
                Id = e.Id,
                EmployeeNumber = e.EmployeeNumber,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                DateOfBirth = e.DateOfBirth,
                HireDate = e.HireDate,
                TerminationDate = e.TerminationDate,
                Address = e.Address,
                City = e.City,
                State = e.State,
                PostalCode = e.PostalCode,
                Country = e.Country,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name,
                PositionId = e.PositionId,
                PositionTitle = e.Position.Title,
                ManagerId = e.ManagerId,
                ManagerName = e.Manager == null ? null : e.Manager.FirstName + " " + e.Manager.LastName,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .ToListAsync();

        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeReadDto>> GetEmployee(int id)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Manager)
            .Where(e => e.Id == id)
            .Select(e => new EmployeeReadDto
            {
                Id = e.Id,
                EmployeeNumber = e.EmployeeNumber,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                DateOfBirth = e.DateOfBirth,
                HireDate = e.HireDate,
                TerminationDate = e.TerminationDate,
                Address = e.Address,
                City = e.City,
                State = e.State,
                PostalCode = e.PostalCode,
                Country = e.Country,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name,
                PositionId = e.PositionId,
                PositionTitle = e.Position.Title,
                ManagerId = e.ManagerId,
                ManagerName = e.Manager == null ? null : e.Manager.FirstName + " " + e.Manager.LastName,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (employee == null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeReadDto>> CreateEmployee(EmployeeCreateDto createDto)
    {
        if (createDto.TerminationDate.HasValue && createDto.TerminationDate < createDto.HireDate)
        {
            return BadRequest("Termination date cannot be before hire date.");
        }

        var employeeNumberExists = await _context.Employees
            .AnyAsync(e => e.EmployeeNumber == createDto.EmployeeNumber);

        if (employeeNumberExists)
        {
            return BadRequest("Employee number already exists.");
        }

        var emailExists = await _context.Employees
            .AnyAsync(e => e.Email == createDto.Email);

        if (emailExists)
        {
            return BadRequest("Employee email already exists.");
        }

        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == createDto.DepartmentId);

        if (department == null)
        {
            return BadRequest("Department does not exist.");
        }

        var position = await _context.Positions
            .FirstOrDefaultAsync(p => p.Id == createDto.PositionId);

        if (position == null)
        {
            return BadRequest("Position does not exist.");
        }

        if (position.DepartmentId != createDto.DepartmentId)
        {
            return BadRequest("Position does not belong to the selected department.");
        }

        Employee? manager = null;

        if (createDto.ManagerId.HasValue)
        {
            manager = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == createDto.ManagerId.Value);

            if (manager == null)
            {
                return BadRequest("Manager does not exist.");
            }
        }

        var employee = new Employee
        {
            EmployeeNumber = createDto.EmployeeNumber,
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Email = createDto.Email,
            Phone = createDto.Phone,
            DateOfBirth = createDto.DateOfBirth,
            HireDate = createDto.HireDate,
            TerminationDate = createDto.TerminationDate,
            Address = createDto.Address,
            City = createDto.City,
            State = createDto.State,
            PostalCode = createDto.PostalCode,
            Country = createDto.Country,
            DepartmentId = createDto.DepartmentId,
            PositionId = createDto.PositionId,
            ManagerId = createDto.ManagerId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        var readDto = new EmployeeReadDto
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
            DepartmentName = department.Name,
            PositionId = employee.PositionId,
            PositionTitle = position.Title,
            ManagerId = employee.ManagerId,
            ManagerName = manager == null ? null : manager.FirstName + " " + manager.LastName,
            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, readDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEmployee(int id, EmployeeUpdateDto updateDto)
    {
        if (updateDto.ManagerId == id)
        {
            return BadRequest("Employee cannot be their own manager.");
        }

        if (updateDto.TerminationDate.HasValue && updateDto.TerminationDate < updateDto.HireDate)
        {
            return BadRequest("Termination date cannot be before hire date.");
        }

        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        var employeeNumberExists = await _context.Employees
            .AnyAsync(e => e.EmployeeNumber == updateDto.EmployeeNumber && e.Id != id);

        if (employeeNumberExists)
        {
            return BadRequest("Employee number already exists.");
        }

        var emailExists = await _context.Employees
            .AnyAsync(e => e.Email == updateDto.Email && e.Id != id);

        if (emailExists)
        {
            return BadRequest("Employee email already exists.");
        }

        var departmentExists = await _context.Departments
            .AnyAsync(d => d.Id == updateDto.DepartmentId);

        if (!departmentExists)
        {
            return BadRequest("Department does not exist.");
        }

        var position = await _context.Positions
            .FirstOrDefaultAsync(p => p.Id == updateDto.PositionId);

        if (position == null)
        {
            return BadRequest("Position does not exist.");
        }

        if (position.DepartmentId != updateDto.DepartmentId)
        {
            return BadRequest("Position does not belong to the selected department.");
        }

        if (updateDto.ManagerId.HasValue)
        {
            var managerExists = await _context.Employees
                .AnyAsync(e => e.Id == updateDto.ManagerId.Value);

            if (!managerExists)
            {
                return BadRequest("Manager does not exist.");
            }
        }

        employee.EmployeeNumber = updateDto.EmployeeNumber;
        employee.FirstName = updateDto.FirstName;
        employee.LastName = updateDto.LastName;
        employee.Email = updateDto.Email;
        employee.Phone = updateDto.Phone;
        employee.DateOfBirth = updateDto.DateOfBirth;
        employee.HireDate = updateDto.HireDate;
        employee.TerminationDate = updateDto.TerminationDate;
        employee.Address = updateDto.Address;
        employee.City = updateDto.City;
        employee.State = updateDto.State;
        employee.PostalCode = updateDto.PostalCode;
        employee.Country = updateDto.Country;
        employee.DepartmentId = updateDto.DepartmentId;
        employee.PositionId = updateDto.PositionId;
        employee.ManagerId = updateDto.ManagerId;
        employee.IsActive = updateDto.IsActive;
        employee.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        var hasSubordinates = await _context.Employees
            .AnyAsync(e => e.ManagerId == id);

        if (hasSubordinates)
        {
            return BadRequest("Cannot delete employee because they manage other employees.");
        }

        var hasLeaveRequests = await _context.LeaveRequests
            .AnyAsync(lr => lr.EmployeeId == id);

        if (hasLeaveRequests)
        {
            return BadRequest("Cannot delete employee because they have leave requests.");
        }

        var hasAttendanceRecords = await _context.Attendances
            .AnyAsync(a => a.EmployeeId == id);

        if (hasAttendanceRecords)
        {
            return BadRequest("Cannot delete employee because they have attendance records.");
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}