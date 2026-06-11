using HRAPI.Data;
using HRAPI.DTOs.Attendances;
using HRAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HRAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendancesController : ControllerBase
{
    private readonly AppDbContext _context;

    public AttendancesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<IEnumerable<AttendanceReadDto>>> GetAttendances()
    {
        var attendances = await _context.Attendances
            .AsNoTracking()
            .Include(a => a.Employee)
            .Select(a => new AttendanceReadDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName,
                Date = a.Date,
                CheckIn = a.CheckIn,
                CheckOut = a.CheckOut,
                Status = a.Status,
                Notes = a.Notes,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync();

        return Ok(attendances);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager,TeamLead,Employee")]
    public async Task<ActionResult<AttendanceReadDto>> GetAttendance(int id)
    {
        var attendance = await _context.Attendances
            .AsNoTracking()
            .Include(a => a.Employee)
            .Where(a => a.Id == id)
            .Select(a => new AttendanceReadDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName,
                Date = a.Date,
                CheckIn = a.CheckIn,
                CheckOut = a.CheckOut,
                Status = a.Status,
                Notes = a.Notes,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (attendance == null)
        {
            return NotFound();
        }

        return Ok(attendance);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<AttendanceReadDto>> CreateAttendance(AttendanceCreateDto createDto)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == createDto.EmployeeId);

        if (employee == null)
        {
            return BadRequest("Employee does not exist.");
        }

        var duplicateExists = await _context.Attendances
            .AnyAsync(a => a.EmployeeId == createDto.EmployeeId && a.Date == createDto.Date);

        if (duplicateExists)
        {
            return BadRequest("Attendance record already exists for this employee and date.");
        }

        var attendance = new Attendance
        {
            EmployeeId = createDto.EmployeeId,
            Date = createDto.Date,
            CheckIn = createDto.CheckIn,
            CheckOut = createDto.CheckOut,
            Status = createDto.Status,
            Notes = createDto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync();

        var readDto = new AttendanceReadDto
        {
            Id = attendance.Id,
            EmployeeId = attendance.EmployeeId,
            EmployeeName = employee.FirstName + " " + employee.LastName,
            Date = attendance.Date,
            CheckIn = attendance.CheckIn,
            CheckOut = attendance.CheckOut,
            Status = attendance.Status,
            Notes = attendance.Notes,
            CreatedAt = attendance.CreatedAt,
            UpdatedAt = attendance.UpdatedAt
        };

        return CreatedAtAction(nameof(GetAttendance), new { id = attendance.Id }, readDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdateAttendance(int id, AttendanceUpdateDto updateDto)
    {
        var attendance = await _context.Attendances.FindAsync(id);

        if (attendance == null)
        {
            return NotFound();
        }

        var employeeExists = await _context.Employees
            .AnyAsync(e => e.Id == updateDto.EmployeeId);

        if (!employeeExists)
        {
            return BadRequest("Employee does not exist.");
        }

        var duplicateExists = await _context.Attendances
            .AnyAsync(a => a.EmployeeId == updateDto.EmployeeId && a.Date == updateDto.Date && a.Id != id);

        if (duplicateExists)
        {
            return BadRequest("Attendance record already exists for this employee and date.");
        }

        attendance.EmployeeId = updateDto.EmployeeId;
        attendance.Date = updateDto.Date;
        attendance.CheckIn = updateDto.CheckIn;
        attendance.CheckOut = updateDto.CheckOut;
        attendance.Status = updateDto.Status;
        attendance.Notes = updateDto.Notes;
        attendance.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAttendance(int id)
    {
        var attendance = await _context.Attendances.FindAsync(id);

        if (attendance == null)
        {
            return NotFound();
        }

        _context.Attendances.Remove(attendance);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
