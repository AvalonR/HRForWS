using HRAPI.DTOs.Positions;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
    private readonly IPositionService _positionService;

    public PositionsController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<IEnumerable<PositionReadDto>>> GetPositions()
    {
        return Ok(await _positionService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<PositionReadDto>> GetPosition(int id)
    {
        var position = await _positionService.GetByIdAsync(id);
        return position == null ? NotFound() : Ok(position);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<PositionReadDto>> CreatePosition(PositionCreateDto createDto)
    {
        var result = await _positionService.CreateAsync(createDto);
        if (!result.Succeeded)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetPosition), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdatePosition(int id, PositionUpdateDto updateDto)
    {
        var result = await _positionService.UpdateAsync(id, updateDto);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePosition(int id)
    {
        var result = await _positionService.DeleteAsync(id);
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
