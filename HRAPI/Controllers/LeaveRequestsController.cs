using HRAPI.Data;
using HRAPI.DTOs.LeaveRequests;
using HRAPI.Enums;
using HRAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveRequestsController : ControllerBase
{
    private readonly AppDbContext _context;

    public LeaveRequestsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeaveRequestReadDto>>> GetLeaveRequests()
    {
        var leaveRequests = await _context.LeaveRequests
            .AsNoTracking()
            .Include(lr => lr.Employee)
            .Include(lr => lr.LeaveType)
            .Include(lr => lr.ReviewedByEmployee)
            .Select(lr => new LeaveRequestReadDto
            {
                Id = lr.Id,
                EmployeeId = lr.EmployeeId,
                EmployeeName = lr.Employee.FirstName + " " + lr.Employee.LastName,
                LeaveTypeId = lr.LeaveTypeId,
                LeaveTypeName = lr.LeaveType.Name,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Status = lr.Status,
                Reason = lr.Reason,
                DateRequested = lr.DateRequested,
                ReviewedByEmployeeId = lr.ReviewedByEmployeeId,
                ReviewedByEmployeeName = lr.ReviewedByEmployee == null ? null : lr.ReviewedByEmployee.FirstName + " " + lr.ReviewedByEmployee.LastName,
                CreatedAt = lr.CreatedAt,
                UpdatedAt = lr.UpdatedAt
            })
            .ToListAsync();

        return Ok(leaveRequests);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LeaveRequestReadDto>> GetLeaveRequest(int id)
    {
        var leaveRequest = await _context.LeaveRequests
            .AsNoTracking()
            .Include(lr => lr.Employee)
            .Include(lr => lr.LeaveType)
            .Include(lr => lr.ReviewedByEmployee)
            .Where(lr => lr.Id == id)
            .Select(lr => new LeaveRequestReadDto
            {
                Id = lr.Id,
                EmployeeId = lr.EmployeeId,
                EmployeeName = lr.Employee.FirstName + " " + lr.Employee.LastName,
                LeaveTypeId = lr.LeaveTypeId,
                LeaveTypeName = lr.LeaveType.Name,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Status = lr.Status,
                Reason = lr.Reason,
                DateRequested = lr.DateRequested,
                ReviewedByEmployeeId = lr.ReviewedByEmployeeId,
                ReviewedByEmployeeName = lr.ReviewedByEmployee == null ? null : lr.ReviewedByEmployee.FirstName + " " + lr.ReviewedByEmployee.LastName,
                CreatedAt = lr.CreatedAt,
                UpdatedAt = lr.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (leaveRequest == null)
        {
            return NotFound();
        }

        return Ok(leaveRequest);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveRequestReadDto>> CreateLeaveRequest(LeaveRequestCreateDto createDto)
    {
        if (createDto.EndDate < createDto.StartDate)
        {
            return BadRequest("End date cannot be before start date.");
        }

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == createDto.EmployeeId);

        if (employee == null)
        {
            return BadRequest("Employee does not exist.");
        }

        var leaveType = await _context.LeaveTypes
            .FirstOrDefaultAsync(lt => lt.Id == createDto.LeaveTypeId);

        if (leaveType == null)
        {
            return BadRequest("Leave type does not exist.");
        }

        var now = DateTime.UtcNow;
        var leaveRequest = new LeaveRequest
        {
            EmployeeId = createDto.EmployeeId,
            LeaveTypeId = createDto.LeaveTypeId,
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
            Status = LeaveRequestStatus.Pending,
            Reason = createDto.Reason,
            DateRequested = now,
            CreatedAt = now
        };

        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync();

        var readDto = new LeaveRequestReadDto
        {
            Id = leaveRequest.Id,
            EmployeeId = leaveRequest.EmployeeId,
            EmployeeName = employee.FirstName + " " + employee.LastName,
            LeaveTypeId = leaveRequest.LeaveTypeId,
            LeaveTypeName = leaveType.Name,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            Status = leaveRequest.Status,
            Reason = leaveRequest.Reason,
            DateRequested = leaveRequest.DateRequested,
            ReviewedByEmployeeId = leaveRequest.ReviewedByEmployeeId,
            ReviewedByEmployeeName = null,
            CreatedAt = leaveRequest.CreatedAt,
            UpdatedAt = leaveRequest.UpdatedAt
        };

        return CreatedAtAction(nameof(GetLeaveRequest), new { id = leaveRequest.Id }, readDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateLeaveRequest(int id, LeaveRequestUpdateDto updateDto)
    {
        if (updateDto.EndDate < updateDto.StartDate)
        {
            return BadRequest("End date cannot be before start date.");
        }

        var leaveRequest = await _context.LeaveRequests.FindAsync(id);

        if (leaveRequest == null)
        {
            return NotFound();
        }

        var employeeExists = await _context.Employees
            .AnyAsync(e => e.Id == updateDto.EmployeeId);

        if (!employeeExists)
        {
            return BadRequest("Employee does not exist.");
        }

        var leaveTypeExists = await _context.LeaveTypes
            .AnyAsync(lt => lt.Id == updateDto.LeaveTypeId);

        if (!leaveTypeExists)
        {
            return BadRequest("Leave type does not exist.");
        }

        if (updateDto.ReviewedByEmployeeId.HasValue)
        {
            var reviewerExists = await _context.Employees
                .AnyAsync(e => e.Id == updateDto.ReviewedByEmployeeId.Value);

            if (!reviewerExists)
            {
                return BadRequest("Reviewer employee does not exist.");
            }
        }

        leaveRequest.EmployeeId = updateDto.EmployeeId;
        leaveRequest.LeaveTypeId = updateDto.LeaveTypeId;
        leaveRequest.StartDate = updateDto.StartDate;
        leaveRequest.EndDate = updateDto.EndDate;
        leaveRequest.Status = updateDto.Status;
        leaveRequest.Reason = updateDto.Reason;
        leaveRequest.ReviewedByEmployeeId = updateDto.ReviewedByEmployeeId;
        leaveRequest.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteLeaveRequest(int id)
    {
        var leaveRequest = await _context.LeaveRequests.FindAsync(id);

        if (leaveRequest == null)
        {
            return NotFound();
        }

        _context.LeaveRequests.Remove(leaveRequest);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
