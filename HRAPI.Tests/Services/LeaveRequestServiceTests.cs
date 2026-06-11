using HRAPI.DTOs.LeaveRequests;
using HRAPI.Enums;
using HRAPI.Models;
using HRAPI.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Tests.Services;

public class LeaveRequestServiceTests
{
    private async Task<(Employee Emp, LeaveType Lt)> SeedLookups(AppDbContext context)
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
        context.Employees.Add(emp);
        var lt = new LeaveType { Name = "Annual", DaysAllowed = 20, IsPaid = true };
        context.LeaveTypes.Add(lt);
        await context.SaveChangesAsync();
        return (emp, lt);
    }

    [Fact]
    public async Task Create_EndBeforeStart_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();

        var service = new LeaveRequestService(context);
        var dto = new LeaveRequestCreateDto
        {
            EmployeeId = 1, LeaveTypeId = 1,
            StartDate = new DateOnly(2024, 6, 10),
            EndDate = new DateOnly(2024, 6, 5),
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("End date cannot be before start date.");
    }

    [Fact]
    public async Task Create_InvalidEmployee_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (_, lt) = await SeedLookups(context);

        var service = new LeaveRequestService(context);
        var dto = new LeaveRequestCreateDto
        {
            EmployeeId = 999, LeaveTypeId = lt.Id,
            StartDate = new DateOnly(2024, 6, 1),
            EndDate = new DateOnly(2024, 6, 5),
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Employee does not exist.");
    }

    [Fact]
    public async Task Create_InvalidLeaveType_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (emp, _) = await SeedLookups(context);

        var service = new LeaveRequestService(context);
        var dto = new LeaveRequestCreateDto
        {
            EmployeeId = emp.Id, LeaveTypeId = 999,
            StartDate = new DateOnly(2024, 6, 1),
            EndDate = new DateOnly(2024, 6, 5),
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Leave type does not exist.");
    }

    [Fact]
    public async Task Create_Valid_Succeeds()
    {
        using var context = DbContextFactory.Create();
        var (emp, lt) = await SeedLookups(context);

        var service = new LeaveRequestService(context);
        var dto = new LeaveRequestCreateDto
        {
            EmployeeId = emp.Id, LeaveTypeId = lt.Id,
            StartDate = new DateOnly(2024, 6, 1),
            EndDate = new DateOnly(2024, 6, 5),
            Reason = "Vacation",
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EmployeeId.Should().Be(emp.Id);
        result.Data.LeaveTypeId.Should().Be(lt.Id);
        result.Data.Status.Should().Be(LeaveRequestStatus.Pending);
        result.Data.Reason.Should().Be("Vacation");
    }

    [Fact]
    public async Task Update_EndBeforeStart_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (emp, lt) = await SeedLookups(context);

        var lr = new LeaveRequest
        {
            EmployeeId = emp.Id, LeaveTypeId = lt.Id,
            StartDate = new DateOnly(2024, 6, 1), EndDate = new DateOnly(2024, 6, 5),
            Status = LeaveRequestStatus.Pending, DateRequested = DateTime.UtcNow,
        };
        context.LeaveRequests.Add(lr);
        await context.SaveChangesAsync();

        var service = new LeaveRequestService(context);
        var dto = new LeaveRequestUpdateDto
        {
            EmployeeId = emp.Id, LeaveTypeId = lt.Id,
            StartDate = new DateOnly(2024, 6, 10), EndDate = new DateOnly(2024, 6, 5),
            Status = LeaveRequestStatus.Pending,
        };

        var result = await service.UpdateAsync(lr.Id, dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("End date cannot be before start date.");
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new LeaveRequestService(context);
        var dto = new LeaveRequestUpdateDto
        {
            EmployeeId = 1, LeaveTypeId = 1,
            StartDate = new DateOnly(2024, 6, 1), EndDate = new DateOnly(2024, 6, 5),
            Status = LeaveRequestStatus.Pending,
        };

        var result = await service.UpdateAsync(999, dto);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Update_InvalidReviewer_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (emp, lt) = await SeedLookups(context);

        var lr = new LeaveRequest
        {
            EmployeeId = emp.Id, LeaveTypeId = lt.Id,
            StartDate = new DateOnly(2024, 6, 1), EndDate = new DateOnly(2024, 6, 5),
            Status = LeaveRequestStatus.Pending, DateRequested = DateTime.UtcNow,
        };
        context.LeaveRequests.Add(lr);
        await context.SaveChangesAsync();

        var service = new LeaveRequestService(context);
        var dto = new LeaveRequestUpdateDto
        {
            EmployeeId = emp.Id, LeaveTypeId = lt.Id,
            StartDate = new DateOnly(2024, 6, 1), EndDate = new DateOnly(2024, 6, 5),
            Status = LeaveRequestStatus.Approved,
            ReviewedByEmployeeId = 999,
        };

        var result = await service.UpdateAsync(lr.Id, dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Reviewer employee does not exist.");
    }

    [Fact]
    public async Task Delete_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new LeaveRequestService(context);

        var result = await service.DeleteAsync(999);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_Valid_Succeeds()
    {
        using var context = DbContextFactory.Create();
        var (emp, lt) = await SeedLookups(context);

        var lr = new LeaveRequest
        {
            EmployeeId = emp.Id, LeaveTypeId = lt.Id,
            StartDate = new DateOnly(2024, 6, 1), EndDate = new DateOnly(2024, 6, 5),
            Status = LeaveRequestStatus.Pending, DateRequested = DateTime.UtcNow,
        };
        context.LeaveRequests.Add(lr);
        await context.SaveChangesAsync();

        var service = new LeaveRequestService(context);

        var result = await service.DeleteAsync(lr.Id);

        result.Succeeded.Should().BeTrue();
        var inDb = await context.LeaveRequests.FindAsync(lr.Id);
        inDb.Should().BeNull();
    }
}
