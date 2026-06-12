using HRAPI.DTOs.PerformanceReviews;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
public class PerformanceReviewsController : ApiControllerBase
{
    private readonly IPerformanceReviewService _performanceReviewService;

    public PerformanceReviewsController(IPerformanceReviewService performanceReviewService)
    {
        _performanceReviewService = performanceReviewService;
    }

    [HttpGet(Name = "GetPerformanceReviews")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<IActionResult> GetPerformanceReviews()
    {
        var items = await _performanceReviewService.GetAllAsync();
        var collectionLinks = Links(
            ("self",   Url?.Link("GetPerformanceReviews", null), "GET"),
            ("create", Url?.Action(nameof(CreatePerformanceReview)), "POST")
        );
        var result = new
        {
            items = items.Select(d => new
            {
                d.Id, d.EmployeeId, d.EmployeeName,
                d.ReviewerId, d.ReviewerName,
                d.ReviewDate, d.Rating,
                d.Strengths, d.AreasForImprovement, d.Goals,
                d.Status, d.NextReviewDate,
                d.CreatedAt, d.UpdatedAt,
                _links = Links(
                    ("self",   Url?.Action(nameof(GetPerformanceReview), new { id = d.Id }), "GET"),
                    ("update", Url?.Action(nameof(UpdatePerformanceReview), new { id = d.Id }), "PUT"),
                    ("delete", Url?.Action(nameof(DeletePerformanceReview), new { id = d.Id }), "DELETE")
                )
            }),
            _links = collectionLinks
        };
        return Ok(result);
    }

    [HttpGet("{id:int}", Name = "GetPerformanceReview")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<IActionResult> GetPerformanceReview(int id)
    {
        var review = await _performanceReviewService.GetByIdAsync(id);
        if (review == null) return NotFound();
        var result = new
        {
            data = review,
            _links = Links(
                ("self",   Url?.Action(nameof(GetPerformanceReview), new { id }), "GET"),
                ("update", Url?.Action(nameof(UpdatePerformanceReview), new { id }), "PUT"),
                ("delete", Url?.Action(nameof(DeletePerformanceReview), new { id }), "DELETE")
            )
        };
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> CreatePerformanceReview(PerformanceReviewCreateDto createDto)
    {
        var result = await _performanceReviewService.CreateAsync(createDto);
        if (!result.Succeeded)
            return BadRequest(result.ErrorMessage);

        var data = result.Data!;
        var resource = new
        {
            data,
            _links = Links(
                ("self",   Url?.Action(nameof(GetPerformanceReview), new { id = data.Id }), "GET"),
                ("update", Url?.Action(nameof(UpdatePerformanceReview), new { id = data.Id }), "PUT"),
                ("delete", Url?.Action(nameof(DeletePerformanceReview), new { id = data.Id }), "DELETE")
            )
        };
        return CreatedAtAction(nameof(GetPerformanceReview), new { id = data.Id }, resource);
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
