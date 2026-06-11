using HRAPI.DTOs.Deductions;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
public class DeductionsController : ApiControllerBase
{
    private readonly IDeductionService _deductionService;

    public DeductionsController(IDeductionService deductionService)
    {
        _deductionService = deductionService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<IEnumerable<DeductionReadDto>>> GetDeductions()
    {
        return Ok(await _deductionService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<DeductionReadDto>> GetDeduction(int id)
    {
        var deduction = await _deductionService.GetByIdAsync(id);
        return deduction == null ? NotFound() : Ok(deduction);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<DeductionReadDto>> CreateDeduction(DeductionCreateDto createDto)
    {
        var result = await _deductionService.CreateAsync(createDto);
        if (!result.Succeeded)
            return BadRequest(result.ErrorMessage);

        return CreatedAtAction(nameof(GetDeduction), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdateDeduction(int id, DeductionUpdateDto updateDto)
    {
        var result = await _deductionService.UpdateAsync(id, updateDto);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDeduction(int id)
    {
        var result = await _deductionService.DeleteAsync(id);
        return ToActionResult(result);
    }
}
