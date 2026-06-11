using HRAPI.DTOs.LeaveTypes;
using HRAPI.Models;
using HRAPI.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Tests.Services;

public class LeaveTypeServiceTests
{
    [Fact]
    public async Task Create_DuplicateName_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        context.LeaveTypes.Add(new LeaveType { Name = "Annual", DaysAllowed = 20, IsPaid = true });
        await context.SaveChangesAsync();

        var service = new LeaveTypeService(context);
        var dto = new LeaveTypeCreateDto { Name = "Annual", DaysAllowed = 15, IsPaid = false };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Leave type name already exists.");
    }

    [Fact]
    public async Task Create_Valid_Succeeds()
    {
        using var context = DbContextFactory.Create();

        var service = new LeaveTypeService(context);
        var dto = new LeaveTypeCreateDto { Name = "Annual", DaysAllowed = 20, IsPaid = true };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Annual");
        result.Data.DaysAllowed.Should().Be(20);
        result.Data.IsPaid.Should().BeTrue();
    }

    [Fact]
    public async Task Update_DuplicateName_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        context.LeaveTypes.AddRange(
            new LeaveType { Name = "Annual", DaysAllowed = 20, IsPaid = true },
            new LeaveType { Name = "Sick", DaysAllowed = 10, IsPaid = true }
        );
        await context.SaveChangesAsync();

        var sick = await context.LeaveTypes.FirstAsync(lt => lt.Name == "Sick");

        var service = new LeaveTypeService(context);
        var dto = new LeaveTypeUpdateDto { Name = "Annual", DaysAllowed = 15, IsPaid = false };

        var result = await service.UpdateAsync(sick.Id, dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Leave type name already exists.");
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new LeaveTypeService(context);
        var dto = new LeaveTypeUpdateDto { Name = "Ghost", DaysAllowed = 5, IsPaid = true };

        var result = await service.UpdateAsync(999, dto);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_UsedByLeaveRequests_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var dept = new Department { Name = "IT", Code = "IT01", IsActive = true };
        context.Departments.Add(dept);
        var pos = new Position { Title = "Dev", DepartmentId = dept.Id, IsActive = true };
        context.Positions.Add(pos);

        var lt = new LeaveType { Name = "Annual", DaysAllowed = 20, IsPaid = true };
        context.LeaveTypes.Add(lt);

        var emp = new Employee
        {
            EmployeeNumber = "EMP01", FirstName = "John", LastName = "Doe",
            Email = "john@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };
        context.Employees.Add(emp);
        await context.SaveChangesAsync();

        context.LeaveRequests.Add(new LeaveRequest
        {
            EmployeeId = emp.Id, LeaveTypeId = lt.Id,
            StartDate = new DateOnly(2024, 6, 1), EndDate = new DateOnly(2024, 6, 5),
            Status = Enums.LeaveRequestStatus.Pending, DateRequested = DateTime.UtcNow,
        });
        await context.SaveChangesAsync();

        var service = new LeaveTypeService(context);

        var result = await service.DeleteAsync(lt.Id);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Cannot delete leave type because it is used by leave requests.");
    }

    [Fact]
    public async Task Delete_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new LeaveTypeService(context);

        var result = await service.DeleteAsync(999);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_Valid_Succeeds()
    {
        using var context = DbContextFactory.Create();
        context.LeaveTypes.Add(new LeaveType { Name = "Annual", DaysAllowed = 20, IsPaid = true });
        await context.SaveChangesAsync();

        var existing = await context.LeaveTypes.FirstAsync();

        var service = new LeaveTypeService(context);

        var result = await service.DeleteAsync(existing.Id);

        result.Succeeded.Should().BeTrue();
        var inDb = await context.LeaveTypes.FindAsync(existing.Id);
        inDb.Should().BeNull();
    }
}
