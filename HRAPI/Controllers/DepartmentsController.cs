using HRAPI.DTOs.Departments;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
// Exposes department CRUD endpoints while DepartmentService handles validation and database work.
public class DepartmentsController : ApiControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet(Name = "GetDepartments")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    [OutputCache(Duration = 60, Tags = ["departments"])]
    public async Task<IActionResult> GetDepartments()
    {
        var items = await _departmentService.GetAllAsync();
        var collectionLinks = Links(
            ("self",   Url?.Link("GetDepartments", null), "GET"),
            ("create", Url?.Action(nameof(CreateDepartment)), "POST")
        );
        var result = new
        {
            items = items.Select(d => new
            {
                d.Id, d.Name, d.Code, d.Description,
                d.ParentDepartmentId, d.IsActive,
                d.CreatedAt, d.UpdatedAt,
                _links = Links(
                    ("self",   Url?.Action(nameof(GetDepartment), new { id = d.Id }), "GET"),
                    ("update", Url?.Action(nameof(UpdateDepartment), new { id = d.Id }), "PUT"),
                    ("delete", Url?.Action(nameof(DeleteDepartment), new { id = d.Id }), "DELETE")
                )
            }),
            _links = collectionLinks
        };
        return Ok(result);
    }

    [HttpGet("{id:int}", Name = "GetDepartment")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    [OutputCache(Duration = 60, Tags = ["departments"])]
    public async Task<IActionResult> GetDepartment(int id)
    {
        var department = await _departmentService.GetByIdAsync(id);
        if (department == null) return NotFound();
        var result = new
        {
            data = department,
            _links = Links(
                ("self",   Url?.Action(nameof(GetDepartment), new { id }), "GET"),
                ("update", Url?.Action(nameof(UpdateDepartment), new { id }), "PUT"),
                ("delete", Url?.Action(nameof(DeleteDepartment), new { id }), "DELETE")
            )
        };
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> CreateDepartment(DepartmentCreateDto createDto)
    {
        var result = await _departmentService.CreateAsync(createDto);
        if (!result.Succeeded)
            return BadRequest(result.ErrorMessage);

        await EvictCacheAsync("departments");
        var data = result.Data!;
        var resource = new
        {
            data,
            _links = Links(
                ("self",   Url?.Action(nameof(GetDepartment), new { id = data.Id }), "GET"),
                ("update", Url?.Action(nameof(UpdateDepartment), new { id = data.Id }), "PUT"),
                ("delete", Url?.Action(nameof(DeleteDepartment), new { id = data.Id }), "DELETE")
            )
        };
        return CreatedAtAction(nameof(GetDepartment), new { id = data.Id }, resource);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdateDepartment(int id, DepartmentUpdateDto updateDto)
    {
        var result = await _departmentService.UpdateAsync(id, updateDto);
        await EvictCacheAsync("departments");
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var result = await _departmentService.DeleteAsync(id);
        await EvictCacheAsync("departments");
        return ToActionResult(result);
    }

}
