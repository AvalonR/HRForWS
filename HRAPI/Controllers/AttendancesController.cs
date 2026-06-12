using HRAPI.DTOs.Attendances;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
public class AttendancesController : ApiControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendancesController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    [HttpGet(Name = "GetAttendances")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<IActionResult> GetAttendances()
    {
        var items = await _attendanceService.GetAllAsync();
        var collectionLinks = Links(
            ("self",   Url?.Link("GetAttendances", null), "GET"),
            ("create", Url?.Action(nameof(CreateAttendance)), "POST")
        );
        var result = new
        {
            items = items.Select(d => new
            {
                d.Id, d.EmployeeId, d.EmployeeName,
                d.Date, d.CheckIn, d.CheckOut,
                d.Status, d.Notes,
                d.CreatedAt, d.UpdatedAt,
                _links = Links(
                    ("self",   Url?.Action(nameof(GetAttendance), new { id = d.Id }), "GET"),
                    ("update", Url?.Action(nameof(UpdateAttendance), new { id = d.Id }), "PUT"),
                    ("delete", Url?.Action(nameof(DeleteAttendance), new { id = d.Id }), "DELETE")
                )
            }),
            _links = collectionLinks
        };
        return Ok(result);
    }

    [HttpGet("{id:int}", Name = "GetAttendance")]
    [Authorize(Roles = "Admin,HRManager,TeamLead,Employee")]
    public async Task<IActionResult> GetAttendance(int id)
    {
        var attendance = await _attendanceService.GetByIdAsync(id);
        if (attendance == null) return NotFound();
        var result = new
        {
            data = attendance,
            _links = Links(
                ("self",   Url?.Action(nameof(GetAttendance), new { id }), "GET"),
                ("update", Url?.Action(nameof(UpdateAttendance), new { id }), "PUT"),
                ("delete", Url?.Action(nameof(DeleteAttendance), new { id }), "DELETE")
            )
        };
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> CreateAttendance(AttendanceCreateDto createDto)
    {
        var result = await _attendanceService.CreateAsync(createDto);
        if (!result.Succeeded)
            return BadRequest(result.ErrorMessage);

        var data = result.Data!;
        var resource = new
        {
            data,
            _links = Links(
                ("self",   Url?.Action(nameof(GetAttendance), new { id = data.Id }), "GET"),
                ("update", Url?.Action(nameof(UpdateAttendance), new { id = data.Id }), "PUT"),
                ("delete", Url?.Action(nameof(DeleteAttendance), new { id = data.Id }), "DELETE")
            )
        };
        return CreatedAtAction(nameof(GetAttendance), new { id = data.Id }, resource);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdateAttendance(int id, AttendanceUpdateDto updateDto)
    {
        var result = await _attendanceService.UpdateAsync(id, updateDto);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAttendance(int id)
    {
        var result = await _attendanceService.DeleteAsync(id);
        return ToActionResult(result);
    }

}
