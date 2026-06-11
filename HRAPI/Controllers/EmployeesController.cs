using System.Security.Claims;
using HRAPI.DTOs.Employees;
using HRAPI.Models;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
// Exposes employee endpoints and keeps employee business rules in EmployeeService.
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly UserManager<AppUser> _userManager;

    public EmployeesController(IEmployeeService employeeService, UserManager<AppUser> userManager)
    {
        _employeeService = employeeService;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<IEnumerable<EmployeeReadDto>>> GetEmployees()
    {
        return Ok(await _employeeService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize]
    // Employees can view themselves, while HR roles can view any employee.
    public async Task<ActionResult<EmployeeReadDto>> GetEmployee(int id)
    {
        var isAdmin = User.IsInRole("Admin");
        var isHr = User.IsInRole("HRManager");
        var isTeamLead = User.IsInRole("TeamLead");
        var currentUser = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email)!);
        var canView = isAdmin || isHr || isTeamLead || currentUser?.EmployeeId == id;

        if (!canView)
        {
            return Forbid();
        }

        var employee = await _employeeService.GetByIdAsync(id);
        return employee == null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<EmployeeReadDto>> CreateEmployee(EmployeeCreateDto createDto)
    {
        var result = await _employeeService.CreateAsync(createDto);
        if (!result.Succeeded)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetEmployee), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdateEmployee(int id, EmployeeUpdateDto updateDto)
    {
        var result = await _employeeService.UpdateAsync(id, updateDto);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var result = await _employeeService.DeleteAsync(id);
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
