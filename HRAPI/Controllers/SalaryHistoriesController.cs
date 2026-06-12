using HRAPI.DTOs.SalaryHistories;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
// Tracks salary changes over time so previous salary records are not overwritten.
public class SalaryHistoriesController : ApiControllerBase
{
    private readonly ISalaryHistoryService _salaryHistoryService;

    public SalaryHistoriesController(ISalaryHistoryService salaryHistoryService)
    {
        _salaryHistoryService = salaryHistoryService;
    }

    [HttpGet(Name = "GetSalaryHistories")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<IActionResult> GetSalaryHistories()
    {
        var items = await _salaryHistoryService.GetAllAsync();
        var collectionLinks = Links(
            ("self",   Url?.Link("GetSalaryHistories", null), "GET"),
            ("create", Url?.Action(nameof(CreateSalaryHistory)), "POST")
        );
        var result = new
        {
            items = items.Select(d => new
            {
                d.Id, d.EmployeeId, d.EmployeeName,
                d.Amount, d.EffectiveFrom, d.EffectiveTo,
                d.ChangeReason, d.CreatedAt, d.UpdatedAt,
                _links = Links(
                    ("self",   Url?.Action(nameof(GetSalaryHistory), new { id = d.Id }), "GET"),
                    ("update", Url?.Action(nameof(UpdateSalaryHistory), new { id = d.Id }), "PUT"),
                    ("delete", Url?.Action(nameof(DeleteSalaryHistory), new { id = d.Id }), "DELETE")
                )
            }),
            _links = collectionLinks
        };
        return Ok(result);
    }

    [HttpGet("{id:int}", Name = "GetSalaryHistory")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<IActionResult> GetSalaryHistory(int id)
    {
        var salaryHistory = await _salaryHistoryService.GetByIdAsync(id);
        if (salaryHistory == null) return NotFound();
        var result = new
        {
            data = salaryHistory,
            _links = Links(
                ("self",   Url?.Action(nameof(GetSalaryHistory), new { id }), "GET"),
                ("update", Url?.Action(nameof(UpdateSalaryHistory), new { id }), "PUT"),
                ("delete", Url?.Action(nameof(DeleteSalaryHistory), new { id }), "DELETE")
            )
        };
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> CreateSalaryHistory(SalaryHistoryCreateDto createDto)
    {
        var result = await _salaryHistoryService.CreateAsync(createDto);
        if (!result.Succeeded)
            return BadRequest(result.ErrorMessage);

        var data = result.Data!;
        var resource = new
        {
            data,
            _links = Links(
                ("self",   Url?.Action(nameof(GetSalaryHistory), new { id = data.Id }), "GET"),
                ("update", Url?.Action(nameof(UpdateSalaryHistory), new { id = data.Id }), "PUT"),
                ("delete", Url?.Action(nameof(DeleteSalaryHistory), new { id = data.Id }), "DELETE")
            )
        };
        return CreatedAtAction(nameof(GetSalaryHistory), new { id = data.Id }, resource);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdateSalaryHistory(int id, SalaryHistoryUpdateDto updateDto)
    {
        var result = await _salaryHistoryService.UpdateAsync(id, updateDto);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteSalaryHistory(int id)
    {
        var result = await _salaryHistoryService.DeleteAsync(id);
        return ToActionResult(result);
    }
}
