using HRAPI.DTOs.SalaryHistories;
using HRAPI.Models;
using HRAPI.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Tests.Services;

public class SalaryHistoryServiceTests
{
    private async Task<Employee> SeedEmployee(AppDbContext context)
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
        await context.SaveChangesAsync();
        return emp;
    }

    [Fact]
    public async Task Create_AmountZero_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var emp = await SeedEmployee(context);

        var service = new SalaryHistoryService(context);
        var dto = new SalaryHistoryCreateDto
        {
            EmployeeId = emp.Id,
            Amount = 0,
            EffectiveFrom = new DateOnly(2024, 1, 1),
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Salary amount must be greater than zero.");
    }

    [Fact]
    public async Task Create_EffectiveToBeforeFrom_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var emp = await SeedEmployee(context);

        var service = new SalaryHistoryService(context);
        var dto = new SalaryHistoryCreateDto
        {
            EmployeeId = emp.Id,
            Amount = 75000,
            EffectiveFrom = new DateOnly(2024, 6, 1),
            EffectiveTo = new DateOnly(2024, 1, 1),
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Effective to date cannot be before effective from date.");
    }

    [Fact]
    public async Task Create_InvalidEmployee_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();

        var service = new SalaryHistoryService(context);
        var dto = new SalaryHistoryCreateDto
        {
            EmployeeId = 999,
            Amount = 75000,
            EffectiveFrom = new DateOnly(2024, 1, 1),
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Employee does not exist.");
    }

    [Fact]
    public async Task Create_Valid_Succeeds()
    {
        using var context = DbContextFactory.Create();
        var emp = await SeedEmployee(context);

        var service = new SalaryHistoryService(context);
        var dto = new SalaryHistoryCreateDto
        {
            EmployeeId = emp.Id,
            Amount = 75000,
            EffectiveFrom = new DateOnly(2024, 1, 1),
            ChangeReason = "Annual raise",
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EmployeeId.Should().Be(emp.Id);
        result.Data.Amount.Should().Be(75000);
        result.Data.ChangeReason.Should().Be("Annual raise");
    }

    [Fact]
    public async Task Update_AmountZero_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var emp = await SeedEmployee(context);

        var sh = new SalaryHistory
        {
            EmployeeId = emp.Id, Amount = 75000,
            EffectiveFrom = new DateOnly(2024, 1, 1),
        };
        context.SalaryHistories.Add(sh);
        await context.SaveChangesAsync();

        var service = new SalaryHistoryService(context);
        var dto = new SalaryHistoryUpdateDto
        {
            EmployeeId = emp.Id, Amount = 0,
            EffectiveFrom = new DateOnly(2024, 1, 1),
        };

        var result = await service.UpdateAsync(sh.Id, dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Salary amount must be greater than zero.");
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new SalaryHistoryService(context);
        var dto = new SalaryHistoryUpdateDto
        {
            EmployeeId = 1, Amount = 75000,
            EffectiveFrom = new DateOnly(2024, 1, 1),
        };

        var result = await service.UpdateAsync(999, dto);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new SalaryHistoryService(context);

        var result = await service.DeleteAsync(999);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_Valid_Succeeds()
    {
        using var context = DbContextFactory.Create();
        var emp = await SeedEmployee(context);

        var sh = new SalaryHistory
        {
            EmployeeId = emp.Id, Amount = 75000,
            EffectiveFrom = new DateOnly(2024, 1, 1),
        };
        context.SalaryHistories.Add(sh);
        await context.SaveChangesAsync();

        var service = new SalaryHistoryService(context);

        var result = await service.DeleteAsync(sh.Id);

        result.Succeeded.Should().BeTrue();
        var inDb = await context.SalaryHistories.FindAsync(sh.Id);
        inDb.Should().BeNull();
    }
}
