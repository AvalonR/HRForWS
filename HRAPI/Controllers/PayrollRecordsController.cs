using HRAPI.DTOs.PayrollRecords;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
public class PayrollRecordsController : ApiControllerBase
{
    private readonly IPayrollRecordService _payrollRecordService;

    public PayrollRecordsController(IPayrollRecordService payrollRecordService)
    {
        _payrollRecordService = payrollRecordService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<IEnumerable<PayrollRecordReadDto>>> GetPayrollRecords()
    {
        return Ok(await _payrollRecordService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<PayrollRecordReadDto>> GetPayrollRecord(int id)
    {
        var payrollRecord = await _payrollRecordService.GetByIdAsync(id);
        return payrollRecord == null ? NotFound() : Ok(payrollRecord);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<PayrollRecordReadDto>> CreatePayrollRecord(PayrollRecordCreateDto createDto)
    {
        var result = await _payrollRecordService.CreateAsync(createDto);
        if (!result.Succeeded)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetPayrollRecord), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdatePayrollRecord(int id, PayrollRecordUpdateDto updateDto)
    {
        var result = await _payrollRecordService.UpdateAsync(id, updateDto);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePayrollRecord(int id)
    {
        var result = await _payrollRecordService.DeleteAsync(id);
        return ToActionResult(result);
    }

}
