using HRAPI.DTOs.Attendances;
using HRAPI.Enums;
using HRAPI.Models;
using HRAPI.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Tests.Services;

public class AttendanceServiceTests
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
    public async Task Create_DuplicateEmployeeDate_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var emp = await SeedEmployee(context);

        context.Attendances.Add(new Attendance
        {
            EmployeeId = emp.Id,
            Date = new DateOnly(2024, 6, 10),
            Status = AttendanceStatus.Present,
        });
        await context.SaveChangesAsync();

        var service = new AttendanceService(context);
        var dto = new AttendanceCreateDto
        {
            EmployeeId = emp.Id,
            Date = new DateOnly(2024, 6, 10),
            Status = AttendanceStatus.Present,
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Attendance record already exists for this employee and date.");
    }

    [Fact]
    public async Task Create_InvalidEmployee_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();

        var service = new AttendanceService(context);
        var dto = new AttendanceCreateDto
        {
            EmployeeId = 999,
            Date = new DateOnly(2024, 6, 10),
            Status = AttendanceStatus.Present,
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

        var service = new AttendanceService(context);
        var dto = new AttendanceCreateDto
        {
            EmployeeId = emp.Id,
            Date = new DateOnly(2024, 6, 10),
            CheckIn = new TimeOnly(9, 0),
            CheckOut = new TimeOnly(17, 0),
            Status = AttendanceStatus.Present,
            Notes = "On time",
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EmployeeId.Should().Be(emp.Id);
        result.Data.Date.Should().Be(new DateOnly(2024, 6, 10));
        result.Data.Status.Should().Be(AttendanceStatus.Present);
    }

    [Fact]
    public async Task Update_DuplicateEmployeeDate_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var emp = await SeedEmployee(context);

        context.Attendances.AddRange(
            new Attendance { EmployeeId = emp.Id, Date = new DateOnly(2024, 6, 10), Status = AttendanceStatus.Present },
            new Attendance { EmployeeId = emp.Id, Date = new DateOnly(2024, 6, 11), Status = AttendanceStatus.Present }
        );
        await context.SaveChangesAsync();

        var first = await context.Attendances.FirstAsync(a => a.Date == new DateOnly(2024, 6, 10));

        var service = new AttendanceService(context);
        var dto = new AttendanceUpdateDto
        {
            EmployeeId = emp.Id,
            Date = new DateOnly(2024, 6, 11),
            Status = AttendanceStatus.Late,
        };

        var result = await service.UpdateAsync(first.Id, dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Attendance record already exists for this employee and date.");
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new AttendanceService(context);
        var dto = new AttendanceUpdateDto
        {
            EmployeeId = 1,
            Date = new DateOnly(2024, 6, 10),
            Status = AttendanceStatus.Present,
        };

        var result = await service.UpdateAsync(999, dto);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new AttendanceService(context);

        var result = await service.DeleteAsync(999);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_Valid_Succeeds()
    {
        using var context = DbContextFactory.Create();
        var emp = await SeedEmployee(context);

        var att = new Attendance
        {
            EmployeeId = emp.Id, Date = new DateOnly(2024, 6, 10),
            Status = AttendanceStatus.Present,
        };
        context.Attendances.Add(att);
        await context.SaveChangesAsync();

        var service = new AttendanceService(context);

        var result = await service.DeleteAsync(att.Id);

        result.Succeeded.Should().BeTrue();
        var inDb = await context.Attendances.FindAsync(att.Id);
        inDb.Should().BeNull();
    }
}
