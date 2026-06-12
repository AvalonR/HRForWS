using System.Security.Claims;
using HRAPI.DTOs.Employees;
using HRAPI.Models;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
public class EmployeesController : ApiControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly UserManager<AppUser> _userManager;

    public EmployeesController(IEmployeeService employeeService, UserManager<AppUser> userManager)
    {
        _employeeService = employeeService;
        _userManager = userManager;
    }

    [HttpGet(Name = "GetEmployees")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<IActionResult> GetEmployees()
    {
        var items = await _employeeService.GetAllAsync();
        var collectionLinks = Links(
            ("self",   Url?.Link("GetEmployees", null), "GET"),
            ("create", Url?.Action(nameof(CreateEmployee)), "POST")
        );
        var result = new
        {
            items = items.Select(d => new
            {
                d.Id, d.EmployeeNumber, d.FirstName, d.LastName, d.FullName,
                d.Email, d.Phone, d.DateOfBirth, d.HireDate, d.TerminationDate,
                d.Address, d.City, d.State, d.PostalCode, d.Country,
                d.DepartmentId, d.DepartmentName,
                d.PositionId, d.PositionTitle,
                d.ManagerId, d.ManagerName,
                d.IsActive, d.CreatedAt, d.UpdatedAt,
                _links = Links(
                    ("self",   Url?.Action(nameof(GetEmployee), new { id = d.Id }), "GET"),
                    ("update", Url?.Action(nameof(UpdateEmployee), new { id = d.Id }), "PUT"),
                    ("delete", Url?.Action(nameof(DeleteEmployee), new { id = d.Id }), "DELETE")
                )
            }),
            _links = collectionLinks
        };
        return Ok(result);
    }

    [HttpGet("{id:int}", Name = "GetEmployee")]
    [Authorize]
    public async Task<IActionResult> GetEmployee(int id)
    {
        var isAdmin = User.IsInRole("Admin");
        var isHr = User.IsInRole("HRManager");
        var isTeamLead = User.IsInRole("TeamLead");
        var currentUser = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email)!);
        var canView = isAdmin || isHr || isTeamLead || currentUser?.EmployeeId == id;

        if (!canView)
            return Forbid();

        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null) return NotFound();
        var result = new
        {
            data = employee,
            _links = Links(
                ("self",   Url?.Action(nameof(GetEmployee), new { id }), "GET"),
                ("update", Url?.Action(nameof(UpdateEmployee), new { id }), "PUT"),
                ("delete", Url?.Action(nameof(DeleteEmployee), new { id }), "DELETE")
            )
        };
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> CreateEmployee(EmployeeCreateDto createDto)
    {
        var result = await _employeeService.CreateAsync(createDto);
        if (!result.Succeeded)
            return BadRequest(result.ErrorMessage);

        var data = result.Data!;
        var resource = new
        {
            data,
            _links = Links(
                ("self",   Url?.Action(nameof(GetEmployee), new { id = data.Id }), "GET"),
                ("update", Url?.Action(nameof(UpdateEmployee), new { id = data.Id }), "PUT"),
                ("delete", Url?.Action(nameof(DeleteEmployee), new { id = data.Id }), "DELETE")
            )
        };
        return CreatedAtAction(nameof(GetEmployee), new { id = data.Id }, resource);
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

}
