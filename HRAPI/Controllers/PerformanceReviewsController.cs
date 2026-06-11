using HRAPI.Data;
using HRAPI.DTOs.PerformanceReviews;
using HRAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HRAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
// Manages employee performance review records and reviewer relationships.
public class PerformanceReviewsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PerformanceReviewsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<IEnumerable<PerformanceReviewReadDto>>> GetPerformanceReviews()
    {
        var reviews = await _context.PerformanceReviews
            .AsNoTracking()
            .Include(pr => pr.Employee)
            .Include(pr => pr.Reviewer)
            .Select(pr => new PerformanceReviewReadDto
            {
                Id = pr.Id,
                EmployeeId = pr.EmployeeId,
                EmployeeName = pr.Employee.FirstName + " " + pr.Employee.LastName,
                ReviewerId = pr.ReviewerId,
                ReviewerName = pr.Reviewer.FirstName + " " + pr.Reviewer.LastName,
                ReviewDate = pr.ReviewDate,
                Rating = pr.Rating,
                Strengths = pr.Strengths,
                AreasForImprovement = pr.AreasForImprovement,
                Goals = pr.Goals,
                Status = pr.Status,
                NextReviewDate = pr.NextReviewDate,
                CreatedAt = pr.CreatedAt,
                UpdatedAt = pr.UpdatedAt
            })
            .ToListAsync();

        return Ok(reviews);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,HRManager,TeamLead")]
    public async Task<ActionResult<PerformanceReviewReadDto>> GetPerformanceReview(int id)
    {
        var review = await _context.PerformanceReviews
            .AsNoTracking()
            .Include(pr => pr.Employee)
            .Include(pr => pr.Reviewer)
            .Where(pr => pr.Id == id)
            .Select(pr => new PerformanceReviewReadDto
            {
                Id = pr.Id,
                EmployeeId = pr.EmployeeId,
                EmployeeName = pr.Employee.FirstName + " " + pr.Employee.LastName,
                ReviewerId = pr.ReviewerId,
                ReviewerName = pr.Reviewer.FirstName + " " + pr.Reviewer.LastName,
                ReviewDate = pr.ReviewDate,
                Rating = pr.Rating,
                Strengths = pr.Strengths,
                AreasForImprovement = pr.AreasForImprovement,
                Goals = pr.Goals,
                Status = pr.Status,
                NextReviewDate = pr.NextReviewDate,
                CreatedAt = pr.CreatedAt,
                UpdatedAt = pr.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (review == null)
        {
            return NotFound();
        }

        return Ok(review);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<PerformanceReviewReadDto>> CreatePerformanceReview(PerformanceReviewCreateDto createDto)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == createDto.EmployeeId);

        if (employee == null)
        {
            return BadRequest("Employee does not exist.");
        }

        var reviewer = await _context.Employees.FirstOrDefaultAsync(e => e.Id == createDto.ReviewerId);

        if (reviewer == null)
        {
            return BadRequest("Reviewer employee does not exist.");
        }

        var review = new PerformanceReview
        {
            EmployeeId = createDto.EmployeeId,
            ReviewerId = createDto.ReviewerId,
            ReviewDate = createDto.ReviewDate,
            Rating = createDto.Rating,
            Strengths = createDto.Strengths,
            AreasForImprovement = createDto.AreasForImprovement,
            Goals = createDto.Goals,
            Status = createDto.Status,
            NextReviewDate = createDto.NextReviewDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.PerformanceReviews.Add(review);
        await _context.SaveChangesAsync();

        var readDto = new PerformanceReviewReadDto
        {
            Id = review.Id,
            EmployeeId = review.EmployeeId,
            EmployeeName = employee.FirstName + " " + employee.LastName,
            ReviewerId = review.ReviewerId,
            ReviewerName = reviewer.FirstName + " " + reviewer.LastName,
            ReviewDate = review.ReviewDate,
            Rating = review.Rating,
            Strengths = review.Strengths,
            AreasForImprovement = review.AreasForImprovement,
            Goals = review.Goals,
            Status = review.Status,
            NextReviewDate = review.NextReviewDate,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };

        return CreatedAtAction(nameof(GetPerformanceReview), new { id = review.Id }, readDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> UpdatePerformanceReview(int id, PerformanceReviewUpdateDto updateDto)
    {
        var review = await _context.PerformanceReviews.FindAsync(id);

        if (review == null)
        {
            return NotFound();
        }

        var employeeExists = await _context.Employees.AnyAsync(e => e.Id == updateDto.EmployeeId);

        if (!employeeExists)
        {
            return BadRequest("Employee does not exist.");
        }

        var reviewerExists = await _context.Employees.AnyAsync(e => e.Id == updateDto.ReviewerId);

        if (!reviewerExists)
        {
            return BadRequest("Reviewer employee does not exist.");
        }

        review.EmployeeId = updateDto.EmployeeId;
        review.ReviewerId = updateDto.ReviewerId;
        review.ReviewDate = updateDto.ReviewDate;
        review.Rating = updateDto.Rating;
        review.Strengths = updateDto.Strengths;
        review.AreasForImprovement = updateDto.AreasForImprovement;
        review.Goals = updateDto.Goals;
        review.Status = updateDto.Status;
        review.NextReviewDate = updateDto.NextReviewDate;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePerformanceReview(int id)
    {
        var review = await _context.PerformanceReviews.FindAsync(id);

        if (review == null)
        {
            return NotFound();
        }

        _context.PerformanceReviews.Remove(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
