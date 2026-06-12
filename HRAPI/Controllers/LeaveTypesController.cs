using HRAPI.DTOs.LeaveTypes;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
// Manages leave type definitions such as annual, sick, and personal leave.
public class LeaveTypesController : ApiControllerBase
{
    private readonly ILeaveTypeService _leaveTypeService;

    public LeaveTypesController(ILeaveTypeService leaveTypeService)
    {
        _leaveTypeService = leaveTypeService;
    }

    [HttpGet(Name = "GetLeaveTypes")]
    [Authorize(Roles = "Admin,HRManager,TeamLead,Employee")]
    [OutputCache(Duration = 60, Tags = ["leaveTypes"])]
    public async Task<IActionResult> GetLeaveTypes()
    {
        var items = await _leaveTypeService.GetAllAsync();
        var collectionLinks = Links(
            ("self",   Url?.Link("GetLeaveTypes", null), "GET"),
            ("create", Url?.Action(nameof(CreateLeaveType)), "POST")
        );
        var result = new
        {
            items = items.Select(d => new
            {
                d.Id, d.Name, d.DaysAllowed, d.IsPaid,
                _links = Links(
                    ("self",   Url?.Action(nameof(GetLeaveType), new { id = d.Id }), "GET"),
                    ("update", Url?.Action(nameof(UpdateLeaveType), new { id = d.Id }), "PUT"),
                    ("delete", Url?.Action(nameof(DeleteLeaveType), new { id = d.Id }), "DELETE")
                )
            }),
            _links = collectionLinks
        };
        return Ok(result);
    }

    [HttpGet("{id:int}", Name = "GetLeaveType")]
    [Authorize(Roles = "Admin,HRManager,TeamLead,Employee")]
    [OutputCache(Duration = 60, Tags = ["leaveTypes"])]
    public async Task<IActionResult> GetLeaveType(int id)
    {
        var leaveType = await _leaveTypeService.GetByIdAsync(id);
        if (leaveType == null) return NotFound();
        var result = new
        {
            data = leaveType,
            _links = Links(
                ("self",   Url?.Action(nameof(GetLeaveType), new { id }), "GET"),
                ("update", Url?.Action(nameof(UpdateLeaveType), new { id }), "PUT"),
                ("delete", Url?.Action(nameof(DeleteLeaveType), new { id }), "DELETE")
            )
        };
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> CreateLeaveType(LeaveTypeCreateDto createDto)
    {
        var result = await _leaveTypeService.CreateAsync(createDto);
        if (!result.Succeeded)
            return BadRequest(result.ErrorMessage);

        await EvictCacheAsync("leaveTypes");
        var data = result.Data!;
        var resource = new
        {
            data,
            _links = Links(
                ("self",   Url?.Action(nameof(GetLeaveType), new { id = data.Id }), "GET"),
                ("update", Url?.Action(nameof(UpdateLeaveType), new { id = data.Id }), "PUT"),
                ("delete", Url?.Action(nameof(DeleteLeaveType), new { id = data.Id }), "DELETE")
            )
        };
        return CreatedAtAction(nameof(GetLeaveType), new { id = data.Id }, resource);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdateLeaveType(int id, LeaveTypeUpdateDto updateDto)
    {
        var result = await _leaveTypeService.UpdateAsync(id, updateDto);
        await EvictCacheAsync("leaveTypes");
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLeaveType(int id)
    {
        var result = await _leaveTypeService.DeleteAsync(id);
        await EvictCacheAsync("leaveTypes");
        return ToActionResult(result);
    }

}
