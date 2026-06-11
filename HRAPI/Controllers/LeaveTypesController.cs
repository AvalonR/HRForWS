using HRAPI.Data;
using HRAPI.DTOs.LeaveTypes;
using HRAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveTypesController : ControllerBase
{
    private readonly AppDbContext _context;

    public LeaveTypesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeaveTypeReadDto>>> GetLeaveTypes()
    {
        var leaveTypes = await _context.LeaveTypes
            .AsNoTracking()
            .Select(lt => new LeaveTypeReadDto
            {
                Id = lt.Id,
                Name = lt.Name,
                DaysAllowed = lt.DaysAllowed,
                IsPaid = lt.IsPaid
            })
            .ToListAsync();

        return Ok(leaveTypes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LeaveTypeReadDto>> GetLeaveType(int id)
    {
        var leaveType = await _context.LeaveTypes
            .AsNoTracking()
            .Where(lt => lt.Id == id)
            .Select(lt => new LeaveTypeReadDto
            {
                Id = lt.Id,
                Name = lt.Name,
                DaysAllowed = lt.DaysAllowed,
                IsPaid = lt.IsPaid
            })
            .FirstOrDefaultAsync();

        if (leaveType == null)
        {
            return NotFound();
        }

        return Ok(leaveType);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveTypeReadDto>> CreateLeaveType(LeaveTypeCreateDto createDto)
    {
        var nameExists = await _context.LeaveTypes
            .AnyAsync(lt => lt.Name == createDto.Name);

        if (nameExists)
        {
            return BadRequest("Leave type name already exists.");
        }

        var leaveType = new LeaveType
        {
            Name = createDto.Name,
            DaysAllowed = createDto.DaysAllowed,
            IsPaid = createDto.IsPaid
        };

        _context.LeaveTypes.Add(leaveType);
        await _context.SaveChangesAsync();

        var readDto = new LeaveTypeReadDto
        {
            Id = leaveType.Id,
            Name = leaveType.Name,
            DaysAllowed = leaveType.DaysAllowed,
            IsPaid = leaveType.IsPaid
        };

        return CreatedAtAction(nameof(GetLeaveType), new { id = leaveType.Id }, readDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateLeaveType(int id, LeaveTypeUpdateDto updateDto)
    {
        var leaveType = await _context.LeaveTypes.FindAsync(id);

        if (leaveType == null)
        {
            return NotFound();
        }

        var nameExists = await _context.LeaveTypes
            .AnyAsync(lt => lt.Name == updateDto.Name && lt.Id != id);

        if (nameExists)
        {
            return BadRequest("Leave type name already exists.");
        }

        leaveType.Name = updateDto.Name;
        leaveType.DaysAllowed = updateDto.DaysAllowed;
        leaveType.IsPaid = updateDto.IsPaid;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteLeaveType(int id)
    {
        var leaveType = await _context.LeaveTypes.FindAsync(id);

        if (leaveType == null)
        {
            return NotFound();
        }

        var hasLeaveRequests = await _context.LeaveRequests
            .AnyAsync(lr => lr.LeaveTypeId == id);

        if (hasLeaveRequests)
        {
            return BadRequest("Cannot delete leave type because it is used by leave requests.");
        }

        _context.LeaveTypes.Remove(leaveType);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}