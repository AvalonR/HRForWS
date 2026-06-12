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

    [HttpGet(Name = "GetDeductions")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> GetDeductions()
    {
        var items = await _deductionService.GetAllAsync();
        var collectionLinks = Links(
            ("self",   Url?.Link("GetDeductions", null), "GET"),
            ("create", Url?.Action(nameof(CreateDeduction)), "POST")
        );
        var result = new
        {
            items = items.Select(d => new
            {
                d.Id, d.PayrollRecordId,
                d.EmployeeId, d.EmployeeName,
                d.Type, d.Amount, d.Description,
                _links = Links(
                    ("self",   Url?.Action(nameof(GetDeduction), new { id = d.Id }), "GET"),
                    ("update", Url?.Action(nameof(UpdateDeduction), new { id = d.Id }), "PUT"),
                    ("delete", Url?.Action(nameof(DeleteDeduction), new { id = d.Id }), "DELETE")
                )
            }),
            _links = collectionLinks
        };
        return Ok(result);
    }

    [HttpGet("{id:int}", Name = "GetDeduction")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> GetDeduction(int id)
    {
        var deduction = await _deductionService.GetByIdAsync(id);
        if (deduction == null) return NotFound();
        var result = new
        {
            data = deduction,
            _links = Links(
                ("self",   Url?.Action(nameof(GetDeduction), new { id }), "GET"),
                ("update", Url?.Action(nameof(UpdateDeduction), new { id }), "PUT"),
                ("delete", Url?.Action(nameof(DeleteDeduction), new { id }), "DELETE")
            )
        };
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> CreateDeduction(DeductionCreateDto createDto)
    {
        var result = await _deductionService.CreateAsync(createDto);
        if (!result.Succeeded)
            return BadRequest(result.ErrorMessage);

        var data = result.Data!;
        var resource = new
        {
            data,
            _links = Links(
                ("self",   Url?.Action(nameof(GetDeduction), new { id = data.Id }), "GET"),
                ("update", Url?.Action(nameof(UpdateDeduction), new { id = data.Id }), "PUT"),
                ("delete", Url?.Action(nameof(DeleteDeduction), new { id = data.Id }), "DELETE")
            )
        };
        return CreatedAtAction(nameof(GetDeduction), new { id = data.Id }, resource);
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
