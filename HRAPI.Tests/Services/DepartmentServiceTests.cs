using HRAPI.DTOs.Departments;
using HRAPI.Models;
using HRAPI.Tests.TestHelpers;
using FluentAssertions;

namespace HRAPI.Tests.Services;

public class DepartmentServiceTests
{
    [Fact]
    public async Task Create_DuplicateCode_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        context.Departments.Add(new Department { Name = "HR", Code = "HR01", IsActive = true });
        await context.SaveChangesAsync();

        var service = new DepartmentService(context);
        var dto = new DepartmentCreateDto { Name = "Human Resources", Code = "HR01" };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Department code already exists.");
    }

    [Fact]
    public async Task Create_InvalidParentId_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();

        var service = new DepartmentService(context);
        var dto = new DepartmentCreateDto { Name = "Child", Code = "CH01", ParentDepartmentId = 999 };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Parent department does not exist.");
    }

    [Fact]
    public async Task Delete_WithSubDepartments_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var parent = new Department { Name = "Parent", Code = "PAR", IsActive = true };
        context.Departments.Add(parent);
        await context.SaveChangesAsync();

        context.Departments.Add(new Department { Name = "Child", Code = "CHD", ParentDepartmentId = parent.Id, IsActive = true });
        await context.SaveChangesAsync();

        var service = new DepartmentService(context);

        var result = await service.DeleteAsync(parent.Id);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Cannot delete department because it has subdepartments.");
    }

    [Fact]
    public async Task Delete_WithEmployees_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var dept = new Department { Name = "IT", Code = "IT01", IsActive = true };
        context.Departments.Add(dept);

        var emp = new Employee
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            EmployeeNumber = "EMP001",
            DepartmentId = dept.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };
        context.Employees.Add(emp);
        await context.SaveChangesAsync();

        var service = new DepartmentService(context);

        var result = await service.DeleteAsync(dept.Id);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Cannot delete department because it has employees.");
    }

    [Fact]
    public async Task Create_ValidData_Succeeds()
    {
        using var context = DbContextFactory.Create();

        var service = new DepartmentService(context);
        var dto = new DepartmentCreateDto { Name = "Finance", Code = "FIN01" };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Finance");
        result.Data.Code.Should().Be("FIN01");
        result.Data.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Update_NonExistingId_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new DepartmentService(context);
        var dto = new DepartmentUpdateDto { Name = "Ghost", Code = "GHT", IsActive = true };

        var result = await service.UpdateAsync(999, dto);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }
}
