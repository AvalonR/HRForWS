using HRAPI.DTOs.LeaveRequests;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
// Handles leave request endpoints while the service validates dates and related employees/types.
public class LeaveRequestsController : ApiControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;

    public LeaveRequestsController(ILeaveRequestService leaveRequestService)
    {
        _leaveRequestService = leaveRequestService;
    }

    [HttpGet(Name = "GetLeaveRequests")]
    [Authorize(Roles = "Admin,HRManager,TeamLead,Employee")]
    public async Task<IActionResult> GetLeaveRequests()
    {
        var items = await _leaveRequestService.GetAllAsync();
        var collectionLinks = Links(
            ("self",   Url?.Link("GetLeaveRequests", null), "GET"),
            ("create", Url?.Action(nameof(CreateLeaveRequest)), "POST")
        );
        var result = new
        {
            items = items.Select(d => new
            {
                d.Id, d.EmployeeId, d.EmployeeName,
                d.LeaveTypeId, d.LeaveTypeName,
                d.StartDate, d.EndDate,
                d.Status, d.Reason, d.DateRequested,
                d.ReviewedByEmployeeId, d.ReviewedByEmployeeName,
                d.CreatedAt, d.UpdatedAt,
                _links = Links(
                    ("self",   Url?.Action(nameof(GetLeaveRequest), new { id = d.Id }), "GET"),
                    ("update", Url?.Action(nameof(UpdateLeaveRequest), new { id = d.Id }), "PUT"),
                    ("delete", Url?.Action(nameof(DeleteLeaveRequest), new { id = d.Id }), "DELETE")
                )
            }),
            _links = collectionLinks
        };
        return Ok(result);
    }

    [HttpGet("{id:int}", Name = "GetLeaveRequest")]
    [Authorize(Roles = "Admin,HRManager,TeamLead,Employee")]
    public async Task<IActionResult> GetLeaveRequest(int id)
    {
        var leaveRequest = await _leaveRequestService.GetByIdAsync(id);
        if (leaveRequest == null) return NotFound();
        var result = new
        {
            data = leaveRequest,
            _links = Links(
                ("self",   Url?.Action(nameof(GetLeaveRequest), new { id }), "GET"),
                ("update", Url?.Action(nameof(UpdateLeaveRequest), new { id }), "PUT"),
                ("delete", Url?.Action(nameof(DeleteLeaveRequest), new { id }), "DELETE")
            )
        };
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager,Employee")]
    public async Task<IActionResult> CreateLeaveRequest(LeaveRequestCreateDto createDto)
    {
        var result = await _leaveRequestService.CreateAsync(createDto);
        if (!result.Succeeded)
            return BadRequest(result.ErrorMessage);

        var data = result.Data!;
        var resource = new
        {
            data,
            _links = Links(
                ("self",   Url?.Action(nameof(GetLeaveRequest), new { id = data.Id }), "GET"),
                ("update", Url?.Action(nameof(UpdateLeaveRequest), new { id = data.Id }), "PUT"),
                ("delete", Url?.Action(nameof(DeleteLeaveRequest), new { id = data.Id }), "DELETE")
            )
        };
        return CreatedAtAction(nameof(GetLeaveRequest), new { id = data.Id }, resource);
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
