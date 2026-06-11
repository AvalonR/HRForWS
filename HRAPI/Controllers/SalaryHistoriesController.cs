using HRAPI.DTOs.SalaryHistories;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
public class SalaryHistoriesController : ApiControllerBase
{
    private readonly ISalaryHistoryService _salaryHistoryService;

    public SalaryHistoriesController(ISalaryHistoryService salaryHistoryService)
    {
        _salaryHistoryService = salaryHistoryService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<IEnumerable<SalaryHistoryReadDto>>> GetSalaryHistories()
    {
        return Ok(await _salaryHistoryService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<SalaryHistoryReadDto>> GetSalaryHistory(int id)
    {
        var salaryHistory = await _salaryHistoryService.GetByIdAsync(id);
        return salaryHistory == null ? NotFound() : Ok(salaryHistory);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<SalaryHistoryReadDto>> CreateSalaryHistory(SalaryHistoryCreateDto createDto)
    {
        var result = await _salaryHistoryService.CreateAsync(createDto);
        if (!result.Succeeded)
            return BadRequest(result.ErrorMessage);

        return CreatedAtAction(nameof(GetSalaryHistory), new { id = result.Data!.Id }, result.Data);
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
