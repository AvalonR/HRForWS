using HRAPI.DTOs.PerformanceReviews;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
// Manages performance reviews and the employee/reviewer relationship.
public class PerformanceReviewsController : ApiControllerBase
{
    private readonly IPerformanceReviewService _performanceReviewService;

    public PerformanceReviewsController(IPerformanceReviewService performanceReviewService)
    {
        _performanceReviewService = performanceReviewService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<IEnumerable<PerformanceReviewReadDto>>> GetPerformanceReviews()
    {
        return Ok(await _performanceReviewService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<PerformanceReviewReadDto>> GetPerformanceReview(int id)
    {
        var review = await _performanceReviewService.GetByIdAsync(id);
        return review == null ? NotFound() : Ok(review);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<PerformanceReviewReadDto>> CreatePerformanceReview(PerformanceReviewCreateDto createDto)
    {
        var result = await _performanceReviewService.CreateAsync(createDto);
        if (!result.Succeeded)
            return BadRequest(result.ErrorMessage);

        return CreatedAtAction(nameof(GetPerformanceReview), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdatePerformanceReview(int id, PerformanceReviewUpdateDto updateDto)
    {
        var result = await _performanceReviewService.UpdateAsync(id, updateDto);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePerformanceReview(int id)
    {
        var result = await _performanceReviewService.DeleteAsync(id);
        return ToActionResult(result);
    }
}
