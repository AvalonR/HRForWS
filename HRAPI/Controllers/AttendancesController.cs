using HRAPI.DTOs.Attendances;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
// Handles attendance endpoints such as present, absent, late, remote, and on-leave records.
public class AttendancesController : ApiControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendancesController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<IEnumerable<AttendanceReadDto>>> GetAttendances()
    {
        return Ok(await _attendanceService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager,TeamLead,Employee")]
    public async Task<ActionResult<AttendanceReadDto>> GetAttendance(int id)
    {
        var attendance = await _attendanceService.GetByIdAsync(id);
        return attendance == null ? NotFound() : Ok(attendance);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<AttendanceReadDto>> CreateAttendance(AttendanceCreateDto createDto)
    {
        var result = await _attendanceService.CreateAsync(createDto);
        if (!result.Succeeded)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetAttendance), new { id = result.Data!.Id }, result.Data);
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
