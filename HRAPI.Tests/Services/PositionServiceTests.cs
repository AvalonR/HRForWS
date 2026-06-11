using HRAPI.DTOs.Positions;
using HRAPI.Models;
using HRAPI.Tests.TestHelpers;
using FluentAssertions;

namespace HRAPI.Tests.Services;

public class PositionServiceTests
{
    [Fact]
    public async Task Create_DuplicateTitleInDepartment_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var dept = new Department { Name = "IT", Code = "IT01", IsActive = true };
        context.Departments.Add(dept);
        await context.SaveChangesAsync();

        context.Positions.Add(new Position { Title = "Developer", DepartmentId = dept.Id, IsActive = true });
        await context.SaveChangesAsync();

        var service = new PositionService(context);
        var dto = new PositionCreateDto { Title = "Developer", DepartmentId = dept.Id };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Position title already exists in this department.");
    }

    [Fact]
    public async Task Create_MinSalaryGreaterThanMaxSalary_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var dept = new Department { Name = "IT", Code = "IT01", IsActive = true };
        context.Departments.Add(dept);
        await context.SaveChangesAsync();

        var service = new PositionService(context);
        var dto = new PositionCreateDto { Title = "Developer", DepartmentId = dept.Id, MinSalary = 80000, MaxSalary = 50000 };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Minimum salary cannot be greater than maximum salary.");
    }

    [Fact]
    public async Task Create_InvalidDepartmentId_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();

        var service = new PositionService(context);
        var dto = new PositionCreateDto { Title = "Developer", DepartmentId = 999 };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Department does not exist.");
    }

    [Fact]
    public async Task Create_ValidData_Succeeds()
    {
        using var context = DbContextFactory.Create();
        var dept = new Department { Name = "IT", Code = "IT01", IsActive = true };
        context.Departments.Add(dept);
        await context.SaveChangesAsync();

        var service = new PositionService(context);
        var dto = new PositionCreateDto { Title = "Developer", Description = "Software dev", DepartmentId = dept.Id, MinSalary = 50000, MaxSalary = 80000 };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("Developer");
        result.Data.DepartmentId.Should().Be(dept.Id);
        result.Data.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Update_NonExistingId_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();
        var dept = new Department { Name = "IT", Code = "IT01", IsActive = true };
        context.Departments.Add(dept);
        await context.SaveChangesAsync();

        var service = new PositionService(context);
        var dto = new PositionUpdateDto { Title = "Ghost", DepartmentId = dept.Id, IsActive = true };

        var result = await service.UpdateAsync(999, dto);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Update_DuplicateTitleInDepartment_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var dept = new Department { Name = "IT", Code = "IT01", IsActive = true };
        context.Departments.Add(dept);
        await context.SaveChangesAsync();

        context.Positions.AddRange(
            new Position { Title = "Developer", DepartmentId = dept.Id, IsActive = true },
            new Position { Title = "Tester", DepartmentId = dept.Id, IsActive = true }
        );
        await context.SaveChangesAsync();

        var existing = await context.Positions.FirstAsync(p => p.Title == "Tester");

        var service = new PositionService(context);
        var dto = new PositionUpdateDto { Title = "Developer", DepartmentId = dept.Id, IsActive = true };

        var result = await service.UpdateAsync(existing.Id, dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Position title already exists in this department.");
    }

    [Fact]
    public async Task Update_InvalidDepartment_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var dept = new Department { Name = "IT", Code = "IT01", IsActive = true };
        context.Departments.Add(dept);
        await context.SaveChangesAsync();

        context.Positions.Add(new Position { Title = "Developer", DepartmentId = dept.Id, IsActive = true });
        await context.SaveChangesAsync();

        var existing = await context.Positions.FirstAsync();

        var service = new PositionService(context);
        var dto = new PositionUpdateDto { Title = "Senior Dev", DepartmentId = 999, IsActive = true };

        var result = await service.UpdateAsync(existing.Id, dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Department does not exist.");
    }

    [Fact]
    public async Task Delete_WithEmployees_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var dept = new Department { Name = "IT", Code = "IT01", IsActive = true };
        context.Departments.Add(dept);

        var pos = new Position { Title = "Developer", DepartmentId = dept.Id, IsActive = true };
        context.Positions.Add(pos);
        await context.SaveChangesAsync();

        context.Employees.Add(new Employee
        {
            FirstName = "John", LastName = "Doe",
            Email = "john@test.com", EmployeeNumber = "EMP001",
            DepartmentId = dept.Id, PositionId = pos.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        });
        await context.SaveChangesAsync();

        var service = new PositionService(context);

        var result = await service.DeleteAsync(pos.Id);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Cannot delete position because it has employees.");
    }

    [Fact]
    public async Task Delete_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new PositionService(context);

        var result = await service.DeleteAsync(999);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_Valid_Succeeds()
    {
        using var context = DbContextFactory.Create();
        var dept = new Department { Name = "IT", Code = "IT01", IsActive = true };
        context.Departments.Add(dept);
        context.Positions.Add(new Position { Title = "Developer", DepartmentId = dept.Id, IsActive = true });
        await context.SaveChangesAsync();

        var existing = await context.Positions.FirstAsync();

        var service = new PositionService(context);

        var result = await service.DeleteAsync(existing.Id);

        result.Succeeded.Should().BeTrue();
        var inDb = await context.Positions.FindAsync(existing.Id);
        inDb.Should().BeNull();
    }
}
