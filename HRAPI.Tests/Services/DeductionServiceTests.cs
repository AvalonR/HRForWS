using HRAPI.DTOs.Deductions;
using HRAPI.Enums;
using HRAPI.Models;
using HRAPI.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Tests.Services;

public class DeductionServiceTests
{
    private async Task<(Employee Emp, PayrollRecord Pr)> SeedPayrollRecord(AppDbContext context)
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

        var pr = new PayrollRecord
        {
            EmployeeId = emp.Id,
            PayPeriodStart = new DateOnly(2024, 6, 1),
            PayPeriodEnd = new DateOnly(2024, 6, 15),
            BaseSalary = 5000, Overtime = 0, Bonuses = 0,
            DeductionsTotal = 0, NetPay = 5000,
            PayDate = new DateOnly(2024, 6, 30),
            Status = PayrollStatus.Pending,
        };
        context.PayrollRecords.Add(pr);
        await context.SaveChangesAsync();
        return (emp, pr);
    }

    [Fact]
    public async Task Create_AmountZero_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (_, pr) = await SeedPayrollRecord(context);

        var service = new DeductionService(context);
        var dto = new DeductionCreateDto
        {
            PayrollRecordId = pr.Id,
            Type = DeductionType.Tax,
            Amount = 0,
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Deduction amount must be greater than zero.");
    }

    [Fact]
    public async Task Create_InvalidPayrollRecord_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();

        var service = new DeductionService(context);
        var dto = new DeductionCreateDto
        {
            PayrollRecordId = 999,
            Type = DeductionType.Tax,
            Amount = 100,
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Payroll record does not exist.");
    }

    [Fact]
    public async Task Create_Valid_Succeeds()
    {
        using var context = DbContextFactory.Create();
        var (emp, pr) = await SeedPayrollRecord(context);

        var service = new DeductionService(context);
        var dto = new DeductionCreateDto
        {
            PayrollRecordId = pr.Id,
            Type = DeductionType.Insurance,
            Amount = 200,
            Description = "Health insurance",
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.PayrollRecordId.Should().Be(pr.Id);
        result.Data.Amount.Should().Be(200);
        result.Data.Type.Should().Be(DeductionType.Insurance);
    }

    [Fact]
    public async Task Update_AmountZero_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var (_, pr) = await SeedPayrollRecord(context);

        var ded = new Deduction
        {
            PayrollRecordId = pr.Id,
            Type = DeductionType.Tax,
            Amount = 100,
        };
        context.Deductions.Add(ded);
        await context.SaveChangesAsync();

        var service = new DeductionService(context);
        var dto = new DeductionUpdateDto
        {
            PayrollRecordId = pr.Id,
            Type = DeductionType.Tax,
            Amount = 0,
        };

        var result = await service.UpdateAsync(ded.Id, dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Deduction amount must be greater than zero.");
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new DeductionService(context);
        var dto = new DeductionUpdateDto
        {
            PayrollRecordId = 1,
            Type = DeductionType.Tax,
            Amount = 100,
        };

        var result = await service.UpdateAsync(999, dto);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new DeductionService(context);

        var result = await service.DeleteAsync(999);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_Valid_Succeeds()
    {
        using var context = DbContextFactory.Create();
        var (_, pr) = await SeedPayrollRecord(context);

        var ded = new Deduction
        {
            PayrollRecordId = pr.Id,
            Type = DeductionType.Tax,
            Amount = 100,
        };
        context.Deductions.Add(ded);
        await context.SaveChangesAsync();

        var service = new DeductionService(context);

        var result = await service.DeleteAsync(ded.Id);

        result.Succeeded.Should().BeTrue();
        var inDb = await context.Deductions.FindAsync(ded.Id);
        inDb.Should().BeNull();
    }
}
