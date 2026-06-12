using HRAPI.DTOs.Positions;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
// Exposes position endpoints and keeps department/duplicate validation inside the service layer.
public class PositionsController : ApiControllerBase
{
    private readonly IPositionService _positionService;

    public PositionsController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    [HttpGet(Name = "GetPositions")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    [OutputCache(Duration = 60, Tags = ["positions"])]
    public async Task<IActionResult> GetPositions()
    {
        var items = await _positionService.GetAllAsync();
        var collectionLinks = Links(
            ("self",   Url?.Link("GetPositions", null), "GET"),
            ("create", Url?.Action(nameof(CreatePosition)), "POST")
        );
        var result = new
        {
            items = items.Select(d => new
            {
                d.Id, d.Title, d.Description,
                d.MinSalary, d.MaxSalary,
                d.DepartmentId, d.DepartmentName,
                d.IsActive, d.CreatedAt, d.UpdatedAt,
                _links = Links(
                    ("self",   Url?.Action(nameof(GetPosition), new { id = d.Id }), "GET"),
                    ("update", Url?.Action(nameof(UpdatePosition), new { id = d.Id }), "PUT"),
                    ("delete", Url?.Action(nameof(DeletePosition), new { id = d.Id }), "DELETE")
                )
            }),
            _links = collectionLinks
        };
        return Ok(result);
    }

    [HttpGet("{id:int}", Name = "GetPosition")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    [OutputCache(Duration = 60, Tags = ["positions"])]
    public async Task<IActionResult> GetPosition(int id)
    {
        var position = await _positionService.GetByIdAsync(id);
        if (position == null) return NotFound();
        var result = new
        {
            data = position,
            _links = Links(
                ("self",   Url?.Action(nameof(GetPosition), new { id }), "GET"),
                ("update", Url?.Action(nameof(UpdatePosition), new { id }), "PUT"),
                ("delete", Url?.Action(nameof(DeletePosition), new { id }), "DELETE")
            )
        };
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> CreatePosition(PositionCreateDto createDto)
    {
        var result = await _positionService.CreateAsync(createDto);
        if (!result.Succeeded)
            return BadRequest(result.ErrorMessage);

        await EvictCacheAsync("positions");
        var data = result.Data!;
        var resource = new
        {
            data,
            _links = Links(
                ("self",   Url?.Action(nameof(GetPosition), new { id = data.Id }), "GET"),
                ("update", Url?.Action(nameof(UpdatePosition), new { id = data.Id }), "PUT"),
                ("delete", Url?.Action(nameof(DeletePosition), new { id = data.Id }), "DELETE")
            )
        };
        return CreatedAtAction(nameof(GetPosition), new { id = data.Id }, resource);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdatePosition(int id, PositionUpdateDto updateDto)
    {
        var result = await _positionService.UpdateAsync(id, updateDto);
        await EvictCacheAsync("positions");
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePosition(int id)
    {
        var result = await _positionService.DeleteAsync(id);
        await EvictCacheAsync("positions");
        return ToActionResult(result);
    }

}
