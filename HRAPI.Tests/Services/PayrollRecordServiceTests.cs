using HRAPI.DTOs.PayrollRecords;
using HRAPI.Enums;
using HRAPI.Models;
using HRAPI.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Tests.Services;

public class PayrollRecordServiceTests
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
    public async Task Create_PayPeriodEndBeforeStart_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var emp = await SeedEmployee(context);

        var service = new PayrollRecordService(context);
        var dto = new PayrollRecordCreateDto
        {
            EmployeeId = emp.Id,
            PayPeriodStart = new DateOnly(2024, 6, 15),
            PayPeriodEnd = new DateOnly(2024, 6, 1),
            BaseSalary = 5000, Overtime = 0, Bonuses = 0,
            DeductionsTotal = 0, NetPay = 5000,
            PayDate = new DateOnly(2024, 6, 30),
            Status = PayrollStatus.Pending,
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Pay period end cannot be before pay period start.");
    }

    [Fact]
    public async Task Create_BaseSalaryZero_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var emp = await SeedEmployee(context);

        var service = new PayrollRecordService(context);
        var dto = new PayrollRecordCreateDto
        {
            EmployeeId = emp.Id,
            PayPeriodStart = new DateOnly(2024, 6, 1),
            PayPeriodEnd = new DateOnly(2024, 6, 15),
            BaseSalary = 0, Overtime = 0, Bonuses = 0,
            DeductionsTotal = 0, NetPay = 0,
            PayDate = new DateOnly(2024, 6, 30),
            Status = PayrollStatus.Pending,
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Base salary must be greater than zero.");
    }

    [Fact]
    public async Task Create_NegativeAmount_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var emp = await SeedEmployee(context);

        var service = new PayrollRecordService(context);
        var dto = new PayrollRecordCreateDto
        {
            EmployeeId = emp.Id,
            PayPeriodStart = new DateOnly(2024, 6, 1),
            PayPeriodEnd = new DateOnly(2024, 6, 15),
            BaseSalary = 5000, Overtime = -100, Bonuses = 0,
            DeductionsTotal = 0, NetPay = 4900,
            PayDate = new DateOnly(2024, 6, 30),
            Status = PayrollStatus.Pending,
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Payroll amounts cannot be negative.");
    }

    [Fact]
    public async Task Create_NetPayMismatch_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var emp = await SeedEmployee(context);

        var service = new PayrollRecordService(context);
        var dto = new PayrollRecordCreateDto
        {
            EmployeeId = emp.Id,
            PayPeriodStart = new DateOnly(2024, 6, 1),
            PayPeriodEnd = new DateOnly(2024, 6, 15),
            BaseSalary = 5000, Overtime = 200, Bonuses = 300,
            DeductionsTotal = 500, NetPay = 9999,
            PayDate = new DateOnly(2024, 6, 30),
            Status = PayrollStatus.Pending,
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Net pay must equal base salary plus overtime and bonuses minus deductions total.");
    }

    [Fact]
    public async Task Create_InvalidEmployee_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();

        var service = new PayrollRecordService(context);
        var dto = new PayrollRecordCreateDto
        {
            EmployeeId = 999,
            PayPeriodStart = new DateOnly(2024, 6, 1),
            PayPeriodEnd = new DateOnly(2024, 6, 15),
            BaseSalary = 5000, Overtime = 0, Bonuses = 0,
            DeductionsTotal = 0, NetPay = 5000,
            PayDate = new DateOnly(2024, 6, 30),
            Status = PayrollStatus.Pending,
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

        var service = new PayrollRecordService(context);
        var dto = new PayrollRecordCreateDto
        {
            EmployeeId = emp.Id,
            PayPeriodStart = new DateOnly(2024, 6, 1),
            PayPeriodEnd = new DateOnly(2024, 6, 15),
            BaseSalary = 5000, Overtime = 200, Bonuses = 300,
            DeductionsTotal = 500, NetPay = 5000,
            PayDate = new DateOnly(2024, 6, 30),
            Status = PayrollStatus.Pending,
        };

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EmployeeId.Should().Be(emp.Id);
        result.Data.NetPay.Should().Be(5000);
        result.Data.Status.Should().Be(PayrollStatus.Pending);
    }

    [Fact]
    public async Task Update_NetPayMismatch_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var emp = await SeedEmployee(context);

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

        var service = new PayrollRecordService(context);
        var dto = new PayrollRecordUpdateDto
        {
            EmployeeId = emp.Id,
            PayPeriodStart = new DateOnly(2024, 6, 1),
            PayPeriodEnd = new DateOnly(2024, 6, 15),
            BaseSalary = 5000, Overtime = 0, Bonuses = 0,
            DeductionsTotal = 0, NetPay = 1,
            PayDate = new DateOnly(2024, 6, 30),
            Status = PayrollStatus.Pending,
        };

        var result = await service.UpdateAsync(pr.Id, dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Net pay must equal base salary plus overtime and bonuses minus deductions total.");
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new PayrollRecordService(context);
        var dto = new PayrollRecordUpdateDto
        {
            EmployeeId = 1,
            PayPeriodStart = new DateOnly(2024, 6, 1),
            PayPeriodEnd = new DateOnly(2024, 6, 15),
            BaseSalary = 5000, Overtime = 0, Bonuses = 0,
            DeductionsTotal = 0, NetPay = 5000,
            PayDate = new DateOnly(2024, 6, 30),
            Status = PayrollStatus.Pending,
        };

        var result = await service.UpdateAsync(999, dto);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_WithDeductions_ReturnsFailure()
    {
        using var context = DbContextFactory.Create();
        var emp = await SeedEmployee(context);

        var pr = new PayrollRecord
        {
            EmployeeId = emp.Id,
            PayPeriodStart = new DateOnly(2024, 6, 1),
            PayPeriodEnd = new DateOnly(2024, 6, 15),
            BaseSalary = 5000, Overtime = 0, Bonuses = 0,
            DeductionsTotal = 100, NetPay = 4900,
            PayDate = new DateOnly(2024, 6, 30),
            Status = PayrollStatus.Pending,
        };
        context.PayrollRecords.Add(pr);
        await context.SaveChangesAsync();

        context.Deductions.Add(new Deduction
        {
            PayrollRecordId = pr.Id,
            Type = DeductionType.Tax,
            Amount = 100,
        });
        await context.SaveChangesAsync();

        var service = new PayrollRecordService(context);

        var result = await service.DeleteAsync(pr.Id);

        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Be("Cannot delete payroll record because it has deductions.");
    }

    [Fact]
    public async Task Delete_NonExisting_ReturnsMissing()
    {
        using var context = DbContextFactory.Create();

        var service = new PayrollRecordService(context);

        var result = await service.DeleteAsync(999);

        result.Succeeded.Should().BeFalse();
        result.NotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_Valid_Succeeds()
    {
        using var context = DbContextFactory.Create();
        var emp = await SeedEmployee(context);

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

        var service = new PayrollRecordService(context);

        var result = await service.DeleteAsync(pr.Id);

        result.Succeeded.Should().BeTrue();
        var inDb = await context.PayrollRecords.FindAsync(pr.Id);
        inDb.Should().BeNull();
    }
}
