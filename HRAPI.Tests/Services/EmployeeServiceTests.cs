using HRAPI.DTOs.Employees;
using HRAPI.Models;
using HRAPI.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Tests.Services;

public class EmployeeServiceTests
{
    private static async Task<(Department Dept, Position Pos)> SeedLookups(AppDbContext context)
    {
        var dept = new Department { Name = "IT", Code = "IT01", IsActive = true };
        context.Departments.Add(dept);
        await context.SaveChangesAsync();

        var pos = new Position { Title = "Developer", DepartmentId = dept.Id, IsActive = true };
        context.Positions.Add(pos);
        await context.SaveChangesAsync();

        return (dept, pos);
    }

    [Fact]
    public async Task Create_DuplicateEmployeeNumber_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (dept, pos) = await SeedLookups(context);

        context.Employees.Add(new Employee
        {
            EmployeeNumber = "EMP001", FirstName = "John", LastName = "Doe",
            Email = "john@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        });
        await context.SaveChangesAsync();

        var service = new EmployeeService(context);
        var dto = new EmployeeCreateDto
        {
            EmployeeNumber = "EMP001", FirstName = "Jane", LastName = "Smith",
            Email = "jane@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Employee number already exists.");
    }

    [Fact]
    public async Task Create_DuplicateEmail_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (dept, pos) = await SeedLookups(context);

        context.Employees.Add(new Employee
        {
            EmployeeNumber = "EMP001", FirstName = "John", LastName = "Doe",
            Email = "dupe@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        });
        await context.SaveChangesAsync();

        var service = new EmployeeService(context);
        var dto = new EmployeeCreateDto
        {
            EmployeeNumber = "EMP002", FirstName = "Jane", LastName = "Smith",
            Email = "dupe@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Employee email already exists.");
    }

    [Fact]
    public async Task Create_TerminationBeforeHire_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (dept, pos) = await SeedLookups(context);

        var service = new EmployeeService(context);
        var dto = new EmployeeCreateDto
        {
            EmployeeNumber = "EMP001", FirstName = "John", LastName = "Doe",
            Email = "john@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            HireDate = new DateOnly(2024, 6, 1),
            TerminationDate = new DateOnly(2023, 1, 1),
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Termination date cannot be before hire date.");
    }

    [Fact]
    public async Task Create_InvalidDepartment_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (_, pos) = await SeedLookups(context);

        var service = new EmployeeService(context);
        var dto = new EmployeeCreateDto
        {
            EmployeeNumber = "EMP001", FirstName = "John", LastName = "Doe",
            Email = "john@test.com", DepartmentId = 999, PositionId = pos.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Department does not exist.");
    }

    [Fact]
    public async Task Create_InvalidPosition_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (dept, _) = await SeedLookups(context);

        var service = new EmployeeService(context);
        var dto = new EmployeeCreateDto
        {
            EmployeeNumber = "EMP001", FirstName = "John", LastName = "Doe",
            Email = "john@test.com", DepartmentId = dept.Id, PositionId = 999,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Position does not exist.");
    }

    [Fact]
    public async Task Create_PositionNotInDepartment_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var hrDept = new Department { Name = "HR", Code = "HR01", IsActive = true };
        context.Departments.Add(hrDept);

        var (itDept, pos) = await SeedLookups(context);
        await context.SaveChangesAsync();

        var service = new EmployeeService(context);
        var dto = new EmployeeCreateDto
        {
            EmployeeNumber = "EMP001", FirstName = "John", LastName = "Doe",
            Email = "john@test.com", DepartmentId = hrDept.Id, PositionId = pos.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Position does not belong to the selected department.");
    }

    [Fact]
    public async Task Create_InvalidManager_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (dept, pos) = await SeedLookups(context);

        var service = new EmployeeService(context);
        var dto = new EmployeeCreateDto
        {
            EmployeeNumber = "EMP001", FirstName = "John", LastName = "Doe",
            Email = "john@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            ManagerId = 999, HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Manager does not exist.");
    }

    [Fact]
    public async Task Create_ValidData_Succeeds()
    {
        using var context = DbContextFactory.Create();
        var (dept, pos) = await SeedLookups(context);

        var service = new EmployeeService(context);
        var dto = new EmployeeCreateDto
        {
            EmployeeNumber = "EMP001", FirstName = "John", LastName = "Doe",
            Email = "john@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            HireDate = new DateOnly(2024, 1, 15),
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EmployeeNumber.Should().Be("EMP001");
        result.Data.Email.Should().Be("john@test.com");
        result.Data.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Update_SelfAsManager_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (dept, pos) = await SeedLookups(context);

        var emp = new Employee
        {
            EmployeeNumber = "EMP001", FirstName = "John", LastName = "Doe",
            Email = "john@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };
        context.Employees.Add(emp);
        await context.SaveChangesAsync();

        var service = new EmployeeService(context);
        var dto = new EmployeeUpdateDto
        {
            EmployeeNumber = "EMP001", FirstName = "John", LastName = "Doe",
            Email = "john@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ManagerId = emp.Id, IsActive = true,
        };

        var result = await service.UpdateAsync(emp.Id, dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Employee cannot be their own manager.");
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();
        var (dept, pos) = await SeedLookups(context);

        var service = new EmployeeService(context);
        var dto = new EmployeeUpdateDto
        {
            EmployeeNumber = "GHOST", FirstName = "Ghost", LastName = "User",
            Email = "ghost@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow), IsActive = true,
        };

        var result = await service.UpdateAsync(999, dto);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_WithSubordinates_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (dept, pos) = await SeedLookups(context);

        var manager = new Employee
        {
            EmployeeNumber = "MGR01", FirstName = "Big", LastName = "Boss",
            Email = "boss@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };
        context.Employees.Add(manager);
        await context.SaveChangesAsync();

        context.Employees.Add(new Employee
        {
            EmployeeNumber = "SUB01", FirstName = "Sub", LastName = "Ordinate",
            Email = "sub@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            ManagerId = manager.Id, HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        });
        await context.SaveChangesAsync();

        var service = new EmployeeService(context);

        var result = await service.DeleteAsync(manager.Id);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Cannot delete employee because they manage other employees.");
    }

    [Fact]
    public async Task Delete_WithLeaveRequests_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (dept, pos) = await SeedLookups(context);

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
            EmployeeId = emp.Id,
            LeaveTypeId = 1,
            StartDate = new DateOnly(2024, 6, 1),
            EndDate = new DateOnly(2024, 6, 5),
            Status = Enums.LeaveRequestStatus.Pending,
            DateRequested = DateTime.UtcNow,
        });
        await context.SaveChangesAsync();

        var service = new EmployeeService(context);

        var result = await service.DeleteAsync(emp.Id);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Cannot delete employee because they have leave requests.");
    }

    [Fact]
    public async Task Delete_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new EmployeeService(context);

        var result = await service.DeleteAsync(999);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_Valid_Succeeds()
    {
        using var context = DbContextFactory.Create();
        var (dept, pos) = await SeedLookups(context);

        var emp = new Employee
        {
            EmployeeNumber = "EMP01", FirstName = "John", LastName = "Doe",
            Email = "john@test.com", DepartmentId = dept.Id, PositionId = pos.Id,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };
        context.Employees.Add(emp);
        await context.SaveChangesAsync();

        var service = new EmployeeService(context);

        var result = await service.DeleteAsync(emp.Id);

        result.Succeeded.Should().BeTrue();
        var inDb = await context.Employees.FindAsync(emp.Id);
        inDb.Should().BeNull();
    }
}
