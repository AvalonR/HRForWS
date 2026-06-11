using HRAPI.DTOs.LeaveTypes;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
// Manages leave type definitions such as annual, sick, or personal leave.
public class LeaveTypesController : ControllerBase
{
    private readonly ILeaveTypeService _leaveTypeService;

    public LeaveTypesController(ILeaveTypeService leaveTypeService)
    {
        _leaveTypeService = leaveTypeService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager,TeamLead,Employee")]
    public async Task<ActionResult<IEnumerable<LeaveTypeReadDto>>> GetLeaveTypes()
    {
        return Ok(await _leaveTypeService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager,TeamLead,Employee")]
    public async Task<ActionResult<LeaveTypeReadDto>> GetLeaveType(int id)
    {
        var leaveType = await _leaveTypeService.GetByIdAsync(id);
        return leaveType == null ? NotFound() : Ok(leaveType);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<LeaveTypeReadDto>> CreateLeaveType(LeaveTypeCreateDto createDto)
    {
        var result = await _leaveTypeService.CreateAsync(createDto);
        if (!result.Succeeded)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetLeaveType), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdateLeaveType(int id, LeaveTypeUpdateDto updateDto)
    {
        var result = await _leaveTypeService.UpdateAsync(id, updateDto);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLeaveType(int id)
    {
        var result = await _leaveTypeService.DeleteAsync(id);
        return ToActionResult(result);
    }

    private IActionResult ToActionResult(ServiceResult result)
    {
        if (result.Succeeded)
        {
            return NoContent();
        }

        return result.NotFound ? NotFound() : BadRequest(result.ErrorMessage);
    }
}
