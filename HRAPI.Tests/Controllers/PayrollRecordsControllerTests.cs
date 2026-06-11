using HRAPI.Controllers;
using HRAPI.DTOs.PayrollRecords;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HRAPI.Tests.Controllers;

public class PayrollRecordsControllerTests
{
    [Fact]
    public async Task GetPayrollRecords_ReturnsOk()
    {
        var mock = new Mock<IPayrollRecordService>();
        mock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PayrollRecordReadDto>());
        var controller = new PayrollRecordsController(mock.Object);

        var result = await controller.GetPayrollRecords();

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPayrollRecord_Existing_ReturnsOk()
    {
        var mock = new Mock<IPayrollRecordService>();
        mock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new PayrollRecordReadDto { Id = 1 });
        var controller = new PayrollRecordsController(mock.Object);

        var result = await controller.GetPayrollRecord(1);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPayrollRecord_NonExisting_ReturnsNotFound()
    {
        var mock = new Mock<IPayrollRecordService>();
        mock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((PayrollRecordReadDto?)null);
        var controller = new PayrollRecordsController(mock.Object);

        var result = await controller.GetPayrollRecord(999);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreatePayrollRecord_Success_ReturnsCreated()
    {
        var dto = new PayrollRecordCreateDto();
        var mock = new Mock<IPayrollRecordService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<PayrollRecordReadDto>.Success(new PayrollRecordReadDto { Id = 1 }));
        var controller = new PayrollRecordsController(mock.Object);

        var result = await controller.CreatePayrollRecord(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task CreatePayrollRecord_Failure_ReturnsBadRequest()
    {
        var dto = new PayrollRecordCreateDto();
        var mock = new Mock<IPayrollRecordService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<PayrollRecordReadDto>.Failure("error"));
        var controller = new PayrollRecordsController(mock.Object);

        var result = await controller.CreatePayrollRecord(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdatePayrollRecord_Success_ReturnsNoContent()
    {
        var dto = new PayrollRecordUpdateDto();
        var mock = new Mock<IPayrollRecordService>();
        mock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(ServiceResult.Success());
        var controller = new PayrollRecordsController(mock.Object);

        var result = await controller.UpdatePayrollRecord(1, dto);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdatePayrollRecord_NotFound_ReturnsNotFound()
    {
        var dto = new PayrollRecordUpdateDto();
        var mock = new Mock<IPayrollRecordService>();
        mock.Setup(s => s.UpdateAsync(999, dto)).ReturnsAsync(ServiceResult.Missing());
        var controller = new PayrollRecordsController(mock.Object);

        var result = await controller.UpdatePayrollRecord(999, dto);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeletePayrollRecord_Success_ReturnsNoContent()
    {
        var mock = new Mock<IPayrollRecordService>();
        mock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(ServiceResult.Success());
        var controller = new PayrollRecordsController(mock.Object);

        var result = await controller.DeletePayrollRecord(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeletePayrollRecord_NotFound_ReturnsNotFound()
    {
        var mock = new Mock<IPayrollRecordService>();
        mock.Setup(s => s.DeleteAsync(999)).ReturnsAsync(ServiceResult.Missing());
        var controller = new PayrollRecordsController(mock.Object);

        var result = await controller.DeletePayrollRecord(999);

        result.Should().BeOfType<NotFoundResult>();
    }
}
