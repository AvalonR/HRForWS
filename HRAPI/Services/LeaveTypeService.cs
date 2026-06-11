using HRAPI.Data;
using HRAPI.DTOs.LeaveTypes;
using HRAPI.Models;
using HRAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Services;

public class LeaveTypeService : ILeaveTypeService
{
    private readonly AppDbContext _context;

    public LeaveTypeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LeaveTypeReadDto>> GetAllAsync()
    {
        return await _context.LeaveTypes.AsNoTracking().Select(lt => ToReadDto(lt)).ToListAsync();
    }

    public async Task<LeaveTypeReadDto?> GetByIdAsync(int id)
    {
        return await _context.LeaveTypes.AsNoTracking().Where(lt => lt.Id == id).Select(lt => ToReadDto(lt)).FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<LeaveTypeReadDto>> CreateAsync(LeaveTypeCreateDto dto)
    {
        var nameExists = await _context.LeaveTypes.AnyAsync(lt => lt.Name == dto.Name);
        if (nameExists)
        {
            return ServiceResult<LeaveTypeReadDto>.Failure("Leave type name already exists.");
        }

        var leaveType = new LeaveType
        {
            Name = dto.Name,
            DaysAllowed = dto.DaysAllowed,
            IsPaid = dto.IsPaid
        };

        _context.LeaveTypes.Add(leaveType);
        await _context.SaveChangesAsync();

        return ServiceResult<LeaveTypeReadDto>.Success(ToReadDto(leaveType));
    }

    public async Task<ServiceResult> UpdateAsync(int id, LeaveTypeUpdateDto dto)
    {
        var leaveType = await _context.LeaveTypes.FindAsync(id);
        if (leaveType == null)
        {
            return ServiceResult.Missing();
        }

        var nameExists = await _context.LeaveTypes.AnyAsync(lt => lt.Name == dto.Name && lt.Id != id);
        if (nameExists)
        {
            return ServiceResult.Failure("Leave type name already exists.");
        }

        leaveType.Name = dto.Name;
        leaveType.DaysAllowed = dto.DaysAllowed;
        leaveType.IsPaid = dto.IsPaid;

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var leaveType = await _context.LeaveTypes.FindAsync(id);
        if (leaveType == null)
        {
            return ServiceResult.Missing();
        }

        var hasLeaveRequests = await _context.LeaveRequests.AnyAsync(lr => lr.LeaveTypeId == id);
        if (hasLeaveRequests)
        {
            return ServiceResult.Failure("Cannot delete leave type because it is used by leave requests.");
        }

        _context.LeaveTypes.Remove(leaveType);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    private static LeaveTypeReadDto ToReadDto(LeaveType leaveType)
    {
        return new LeaveTypeReadDto
        {
            Id = leaveType.Id,
            Name = leaveType.Name,
            DaysAllowed = leaveType.DaysAllowed,
            IsPaid = leaveType.IsPaid
        };
    }
}
