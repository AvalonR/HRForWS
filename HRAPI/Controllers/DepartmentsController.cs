using HRAPI.DTOs.Departments;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<IEnumerable<DepartmentReadDto>>> GetDepartments()
    {
        return Ok(await _departmentService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<DepartmentReadDto>> GetDepartment(int id)
    {
        var department = await _departmentService.GetByIdAsync(id);
        return department == null ? NotFound() : Ok(department);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<DepartmentReadDto>> CreateDepartment(DepartmentCreateDto createDto)
    {
        var result = await _departmentService.CreateAsync(createDto);
        if (!result.Succeeded)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetDepartment), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdateDepartment(int id, DepartmentUpdateDto updateDto)
    {
        var result = await _departmentService.UpdateAsync(id, updateDto);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var result = await _departmentService.DeleteAsync(id);
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
