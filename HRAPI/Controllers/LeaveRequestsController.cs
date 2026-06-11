using HRAPI.DTOs.LeaveRequests;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
public class LeaveRequestsController : ApiControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;

    public LeaveRequestsController(ILeaveRequestService leaveRequestService)
    {
        _leaveRequestService = leaveRequestService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager,TeamLead,Employee")]
    public async Task<ActionResult<IEnumerable<LeaveRequestReadDto>>> GetLeaveRequests()
    {
        return Ok(await _leaveRequestService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager,TeamLead,Employee")]
    public async Task<ActionResult<LeaveRequestReadDto>> GetLeaveRequest(int id)
    {
        var leaveRequest = await _leaveRequestService.GetByIdAsync(id);
        return leaveRequest == null ? NotFound() : Ok(leaveRequest);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager,Employee")]
    public async Task<ActionResult<LeaveRequestReadDto>> CreateLeaveRequest(LeaveRequestCreateDto createDto)
    {
        var result = await _leaveRequestService.CreateAsync(createDto);
        if (!result.Succeeded)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetLeaveRequest), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdateLeaveRequest(int id, LeaveRequestUpdateDto updateDto)
    {
        var result = await _leaveRequestService.UpdateAsync(id, updateDto);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLeaveRequest(int id)
    {
        var result = await _leaveRequestService.DeleteAsync(id);
        return ToActionResult(result);
    }

}
