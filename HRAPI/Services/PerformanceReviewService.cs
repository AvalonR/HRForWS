using HRAPI.Data;
using HRAPI.DTOs.PerformanceReviews;
using HRAPI.Models;
using HRAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Services;

// Handles performance review validation for employee and reviewer relationships.
public class PerformanceReviewService : IPerformanceReviewService
{
    private readonly AppDbContext _context;

    public PerformanceReviewService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PerformanceReviewReadDto>> GetAllAsync()
    {
        return await _context.PerformanceReviews
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
    }

    public async Task<PerformanceReviewReadDto?> GetByIdAsync(int id)
    {
        return await _context.PerformanceReviews
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
    }

    public async Task<ServiceResult<PerformanceReviewReadDto>> CreateAsync(PerformanceReviewCreateDto dto)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == dto.EmployeeId);
        if (employee == null)
            return ServiceResult<PerformanceReviewReadDto>.Failure("Employee does not exist.");

        var reviewer = await _context.Employees.FirstOrDefaultAsync(e => e.Id == dto.ReviewerId);
        if (reviewer == null)
            return ServiceResult<PerformanceReviewReadDto>.Failure("Reviewer employee does not exist.");

        var review = new PerformanceReview
        {
            EmployeeId = dto.EmployeeId,
            ReviewerId = dto.ReviewerId,
            ReviewDate = dto.ReviewDate,
            Rating = dto.Rating,
            Strengths = dto.Strengths,
            AreasForImprovement = dto.AreasForImprovement,
            Goals = dto.Goals,
            Status = dto.Status,
            NextReviewDate = dto.NextReviewDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.PerformanceReviews.Add(review);
        await _context.SaveChangesAsync();

        return ServiceResult<PerformanceReviewReadDto>.Success(new PerformanceReviewReadDto
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
        });
    }

    public async Task<ServiceResult> UpdateAsync(int id, PerformanceReviewUpdateDto dto)
    {
        var review = await _context.PerformanceReviews.FindAsync(id);
        if (review == null)
            return ServiceResult.Missing();

        var employeeExists = await _context.Employees.AnyAsync(e => e.Id == dto.EmployeeId);
        if (!employeeExists)
            return ServiceResult.Failure("Employee does not exist.");

        var reviewerExists = await _context.Employees.AnyAsync(e => e.Id == dto.ReviewerId);
        if (!reviewerExists)
            return ServiceResult.Failure("Reviewer employee does not exist.");

        review.EmployeeId = dto.EmployeeId;
        review.ReviewerId = dto.ReviewerId;
        review.ReviewDate = dto.ReviewDate;
        review.Rating = dto.Rating;
        review.Strengths = dto.Strengths;
        review.AreasForImprovement = dto.AreasForImprovement;
        review.Goals = dto.Goals;
        review.Status = dto.Status;
        review.NextReviewDate = dto.NextReviewDate;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var review = await _context.PerformanceReviews.FindAsync(id);
        if (review == null)
            return ServiceResult.Missing();

        _context.PerformanceReviews.Remove(review);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }
}
