using HRAPI.DTOs.PayrollRecords;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
// Manages payroll records and delegates payroll amount validation to the service layer.
public class PayrollRecordsController : ApiControllerBase
{
    private readonly IPayrollRecordService _payrollRecordService;

    public PayrollRecordsController(IPayrollRecordService payrollRecordService)
    {
        _payrollRecordService = payrollRecordService;
    }

    [HttpGet(Name = "GetPayrollRecords")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> GetPayrollRecords()
    {
        var items = await _payrollRecordService.GetAllAsync();
        var collectionLinks = Links(
            ("self",   Url?.Link("GetPayrollRecords", null), "GET"),
            ("create", Url?.Action(nameof(CreatePayrollRecord)), "POST")
        );
        var result = new
        {
            items = items.Select(d => new
            {
                d.Id, d.EmployeeId, d.EmployeeName,
                d.PayPeriodStart, d.PayPeriodEnd,
                d.BaseSalary, d.Overtime, d.Bonuses,
                d.DeductionsTotal, d.NetPay,
                d.PayDate, d.Status,
                d.CreatedAt, d.UpdatedAt,
                _links = Links(
                    ("self",   Url?.Action(nameof(GetPayrollRecord), new { id = d.Id }), "GET"),
                    ("update", Url?.Action(nameof(UpdatePayrollRecord), new { id = d.Id }), "PUT"),
                    ("delete", Url?.Action(nameof(DeletePayrollRecord), new { id = d.Id }), "DELETE")
                )
            }),
            _links = collectionLinks
        };
        return Ok(result);
    }

    [HttpGet("{id:int}", Name = "GetPayrollRecord")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> GetPayrollRecord(int id)
    {
        var payrollRecord = await _payrollRecordService.GetByIdAsync(id);
        if (payrollRecord == null) return NotFound();
        var result = new
        {
            data = payrollRecord,
            _links = Links(
                ("self",   Url?.Action(nameof(GetPayrollRecord), new { id }), "GET"),
                ("update", Url?.Action(nameof(UpdatePayrollRecord), new { id }), "PUT"),
                ("delete", Url?.Action(nameof(DeletePayrollRecord), new { id }), "DELETE")
            )
        };
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> CreatePayrollRecord(PayrollRecordCreateDto createDto)
    {
        var result = await _payrollRecordService.CreateAsync(createDto);
        if (!result.Succeeded)
            return BadRequest(result.ErrorMessage);

        var data = result.Data!;
        var resource = new
        {
            data,
            _links = Links(
                ("self",   Url?.Action(nameof(GetPayrollRecord), new { id = data.Id }), "GET"),
                ("update", Url?.Action(nameof(UpdatePayrollRecord), new { id = data.Id }), "PUT"),
                ("delete", Url?.Action(nameof(DeletePayrollRecord), new { id = data.Id }), "DELETE")
            )
        };
        return CreatedAtAction(nameof(GetPayrollRecord), new { id = data.Id }, resource);
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
