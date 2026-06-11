using HRAPI.DTOs.PerformanceReviews;
using HRAPI.Enums;
using HRAPI.Models;
using HRAPI.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Tests.Services;

public class PerformanceReviewServiceTests
{
    private async Task<(Employee Emp, Employee Reviewer)> SeedEmployees(AppDbContext context)
    {
        var dept = new Department { Name = "IT", Code = "IT01", IsActive = true };
        context.Departments.Add(dept);
        var pos = new Position { Title = "Dev", DepartmentId = dept.Id, IsActive = true };
        context.Positions.Add(pos);

        var emp = new Employee
        {
            EmployeeNumber = "EMP01", FirstName = "John", LastName = "Doe",
            Email = "john@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };
        var reviewer = new Employee
        {
            EmployeeNumber = "EMP02", FirstName = "Jane", LastName = "Smith",
            Email = "jane@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };
        context.Employees.AddRange(emp, reviewer);
        await context.SaveChangesAsync();
        return (emp, reviewer);
    }

    [Fact]
    public async Task Create_InvalidEmployee_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (_, reviewer) = await SeedEmployees(context);

        var service = new PerformanceReviewService(context);
        var dto = new PerformanceReviewCreateDto
        {
            EmployeeId = 999,
            ReviewerId = reviewer.Id,
            ReviewDate = new DateOnly(2024, 6, 15),
            Rating = 4,
            Status = ReviewStatus.Completed,
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Employee does not exist.");
    }

    [Fact]
    public async Task Create_InvalidReviewer_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (emp, _) = await SeedEmployees(context);

        var service = new PerformanceReviewService(context);
        var dto = new PerformanceReviewCreateDto
        {
            EmployeeId = emp.Id,
            ReviewerId = 999,
            ReviewDate = new DateOnly(2024, 6, 15),
            Rating = 4,
            Status = ReviewStatus.Completed,
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Reviewer employee does not exist.");
    }

    [Fact]
    public async Task Create_Valid_Succeeds()
    {
        using var context = DbContextFactory.Create();
        var (emp, reviewer) = await SeedEmployees(context);

        var service = new PerformanceReviewService(context);
        var dto = new PerformanceReviewCreateDto
        {
            EmployeeId = emp.Id,
            ReviewerId = reviewer.Id,
            ReviewDate = new DateOnly(2024, 6, 15),
            Rating = 4,
            Strengths = "Great work",
            AreasForImprovement = "None",
            Goals = "Keep it up",
            Status = ReviewStatus.Completed,
            NextReviewDate = new DateOnly(2024, 12, 15),
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EmployeeId.Should().Be(emp.Id);
        result.Data.ReviewerId.Should().Be(reviewer.Id);
        result.Data.Rating.Should().Be(4);
        result.Data.Status.Should().Be(ReviewStatus.Completed);
    }

    [Fact]
    public async Task Update_InvalidEmployee_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (emp, reviewer) = await SeedEmployees(context);

        var pr = new PerformanceReview
        {
            EmployeeId = emp.Id, ReviewerId = reviewer.Id,
            ReviewDate = new DateOnly(2024, 6, 15),
            Rating = 3, Status = ReviewStatus.Draft,
        };
        context.PerformanceReviews.Add(pr);
        await context.SaveChangesAsync();

        var service = new PerformanceReviewService(context);
        var dto = new PerformanceReviewUpdateDto
        {
            EmployeeId = 999,
            ReviewerId = reviewer.Id,
            ReviewDate = new DateOnly(2024, 6, 15),
            Rating = 4,
            Status = ReviewStatus.Completed,
        };

        var result = await service.UpdateAsync(pr.Id, dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Employee does not exist.");
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new PerformanceReviewService(context);
        var dto = new PerformanceReviewUpdateDto
        {
            EmployeeId = 1, ReviewerId = 1,
            ReviewDate = new DateOnly(2024, 6, 15),
            Rating = 4, Status = ReviewStatus.Completed,
        };

        var result = await service.UpdateAsync(999, dto);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new PerformanceReviewService(context);

        var result = await service.DeleteAsync(999);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_Valid_Succeeds()
    {
        using var context = DbContextFactory.Create();
        var (emp, reviewer) = await SeedEmployees(context);

        var pr = new PerformanceReview
        {
            EmployeeId = emp.Id, ReviewerId = reviewer.Id,
            ReviewDate = new DateOnly(2024, 6, 15),
            Rating = 3, Status = ReviewStatus.Draft,
        };
        context.PerformanceReviews.Add(pr);
        await context.SaveChangesAsync();

        var service = new PerformanceReviewService(context);

        var result = await service.DeleteAsync(pr.Id);

        result.Succeeded.Should().BeTrue();
        var inDb = await context.PerformanceReviews.FindAsync(pr.Id);
        inDb.Should().BeNull();
    }
}
