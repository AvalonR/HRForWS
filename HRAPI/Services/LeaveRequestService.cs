using HRAPI.Data;
using HRAPI.DTOs.LeaveRequests;
using HRAPI.Enums;
using HRAPI.Models;
using HRAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly AppDbContext _context;

    public LeaveRequestService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LeaveRequestReadDto>> GetAllAsync()
    {
        return await _context.LeaveRequests
            .AsNoTracking()
            .Include(lr => lr.Employee)
            .Include(lr => lr.LeaveType)
            .Include(lr => lr.ReviewedByEmployee)
            .Select(lr => ToReadDto(lr))
            .ToListAsync();
    }

    public async Task<LeaveRequestReadDto?> GetByIdAsync(int id)
    {
        return await _context.LeaveRequests
            .AsNoTracking()
            .Include(lr => lr.Employee)
            .Include(lr => lr.LeaveType)
            .Include(lr => lr.ReviewedByEmployee)
            .Where(lr => lr.Id == id)
            .Select(lr => ToReadDto(lr))
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<LeaveRequestReadDto>> CreateAsync(LeaveRequestCreateDto dto)
    {
        if (dto.EndDate < dto.StartDate)
        {
            return ServiceResult<LeaveRequestReadDto>.Failure("End date cannot be before start date.");
        }

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == dto.EmployeeId);
        if (employee == null)
        {
            return ServiceResult<LeaveRequestReadDto>.Failure("Employee does not exist.");
        }

        var leaveType = await _context.LeaveTypes.FirstOrDefaultAsync(lt => lt.Id == dto.LeaveTypeId);
        if (leaveType == null)
        {
            return ServiceResult<LeaveRequestReadDto>.Failure("Leave type does not exist.");
        }

        var now = DateTime.UtcNow;
        var leaveRequest = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            LeaveTypeId = dto.LeaveTypeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = LeaveRequestStatus.Pending,
            Reason = dto.Reason,
            DateRequested = now,
            CreatedAt = now
        };

        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync();

        leaveRequest.Employee = employee;
        leaveRequest.LeaveType = leaveType;

        return ServiceResult<LeaveRequestReadDto>.Success(ToReadDto(leaveRequest));
    }

    public async Task<ServiceResult> UpdateAsync(int id, LeaveRequestUpdateDto dto)
    {
        if (dto.EndDate < dto.StartDate)
        {
            return ServiceResult.Failure("End date cannot be before start date.");
        }

        var leaveRequest = await _context.LeaveRequests.FindAsync(id);
        if (leaveRequest == null)
        {
            return ServiceResult.Missing();
        }

        var employeeExists = await _context.Employees.AnyAsync(e => e.Id == dto.EmployeeId);
        if (!employeeExists)
        {
            return ServiceResult.Failure("Employee does not exist.");
        }

        var leaveTypeExists = await _context.LeaveTypes.AnyAsync(lt => lt.Id == dto.LeaveTypeId);
        if (!leaveTypeExists)
        {
            return ServiceResult.Failure("Leave type does not exist.");
        }

        if (dto.ReviewedByEmployeeId.HasValue)
        {
            var reviewerExists = await _context.Employees.AnyAsync(e => e.Id == dto.ReviewedByEmployeeId.Value);
            if (!reviewerExists)
            {
                return ServiceResult.Failure("Reviewer employee does not exist.");
            }
        }

        leaveRequest.EmployeeId = dto.EmployeeId;
        leaveRequest.LeaveTypeId = dto.LeaveTypeId;
        leaveRequest.StartDate = dto.StartDate;
        leaveRequest.EndDate = dto.EndDate;
        leaveRequest.Status = dto.Status;
        leaveRequest.Reason = dto.Reason;
        leaveRequest.ReviewedByEmployeeId = dto.ReviewedByEmployeeId;
        leaveRequest.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var leaveRequest = await _context.LeaveRequests.FindAsync(id);
        if (leaveRequest == null)
        {
            return ServiceResult.Missing();
        }

        _context.LeaveRequests.Remove(leaveRequest);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    private static LeaveRequestReadDto ToReadDto(LeaveRequest leaveRequest)
    {
        return new LeaveRequestReadDto
        {
            Id = leaveRequest.Id,
            EmployeeId = leaveRequest.EmployeeId,
            EmployeeName = leaveRequest.Employee.FirstName + " " + leaveRequest.Employee.LastName,
            LeaveTypeId = leaveRequest.LeaveTypeId,
            LeaveTypeName = leaveRequest.LeaveType.Name,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            Status = leaveRequest.Status,
            Reason = leaveRequest.Reason,
            DateRequested = leaveRequest.DateRequested,
            ReviewedByEmployeeId = leaveRequest.ReviewedByEmployeeId,
            ReviewedByEmployeeName = leaveRequest.ReviewedByEmployee == null ? null : leaveRequest.ReviewedByEmployee.FirstName + " " + leaveRequest.ReviewedByEmployee.LastName,
            CreatedAt = leaveRequest.CreatedAt,
            UpdatedAt = leaveRequest.UpdatedAt
        };
    }
}
