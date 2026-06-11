using HRAPI.Data;
using HRAPI.DTOs.Attendances;
using HRAPI.Models;
using HRAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Services;

public class AttendanceService : IAttendanceService
{
    private readonly AppDbContext _context;

    public AttendanceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AttendanceReadDto>> GetAllAsync()
    {
        return await _context.Attendances
            .AsNoTracking()
            .Include(a => a.Employee)
            .Select(a => ToReadDto(a))
            .ToListAsync();
    }

    public async Task<AttendanceReadDto?> GetByIdAsync(int id)
    {
        return await _context.Attendances
            .AsNoTracking()
            .Include(a => a.Employee)
            .Where(a => a.Id == id)
            .Select(a => ToReadDto(a))
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<AttendanceReadDto>> CreateAsync(AttendanceCreateDto dto)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == dto.EmployeeId);
        if (employee == null)
        {
            return ServiceResult<AttendanceReadDto>.Failure("Employee does not exist.");
        }

        var duplicateExists = await _context.Attendances.AnyAsync(a => a.EmployeeId == dto.EmployeeId && a.Date == dto.Date);
        if (duplicateExists)
        {
            return ServiceResult<AttendanceReadDto>.Failure("Attendance record already exists for this employee and date.");
        }

        var attendance = new Attendance
        {
            EmployeeId = dto.EmployeeId,
            Date = dto.Date,
            CheckIn = dto.CheckIn,
            CheckOut = dto.CheckOut,
            Status = dto.Status,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync();

        attendance.Employee = employee;
        return ServiceResult<AttendanceReadDto>.Success(ToReadDto(attendance));
    }

    public async Task<ServiceResult> UpdateAsync(int id, AttendanceUpdateDto dto)
    {
        var attendance = await _context.Attendances.FindAsync(id);
        if (attendance == null)
        {
            return ServiceResult.Missing();
        }

        var employeeExists = await _context.Employees.AnyAsync(e => e.Id == dto.EmployeeId);
        if (!employeeExists)
        {
            return ServiceResult.Failure("Employee does not exist.");
        }

        var duplicateExists = await _context.Attendances.AnyAsync(a => a.EmployeeId == dto.EmployeeId && a.Date == dto.Date && a.Id != id);
        if (duplicateExists)
        {
            return ServiceResult.Failure("Attendance record already exists for this employee and date.");
        }

        attendance.EmployeeId = dto.EmployeeId;
        attendance.Date = dto.Date;
        attendance.CheckIn = dto.CheckIn;
        attendance.CheckOut = dto.CheckOut;
        attendance.Status = dto.Status;
        attendance.Notes = dto.Notes;
        attendance.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var attendance = await _context.Attendances.FindAsync(id);
        if (attendance == null)
        {
            return ServiceResult.Missing();
        }

        _context.Attendances.Remove(attendance);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    private static AttendanceReadDto ToReadDto(Attendance attendance)
    {
        return new AttendanceReadDto
        {
            Id = attendance.Id,
            EmployeeId = attendance.EmployeeId,
            EmployeeName = attendance.Employee.FirstName + " " + attendance.Employee.LastName,
            Date = attendance.Date,
            CheckIn = attendance.CheckIn,
            CheckOut = attendance.CheckOut,
            Status = attendance.Status,
            Notes = attendance.Notes,
            CreatedAt = attendance.CreatedAt,
            UpdatedAt = attendance.UpdatedAt
        };
    }
}
