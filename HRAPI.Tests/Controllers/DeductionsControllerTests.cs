using HRAPI.Controllers;
using HRAPI.DTOs.Deductions;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HRAPI.Tests.Controllers;

public class DeductionsControllerTests
{
    [Fact]
    public async Task GetDeductions_ReturnsOk()
    {
        var mock = new Mock<IDeductionService>();
        mock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<DeductionReadDto>());
        var controller = new DeductionsController(mock.Object);

        var result = await controller.GetDeductions();

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetDeduction_Existing_ReturnsOk()
    {
        var mock = new Mock<IDeductionService>();
        mock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new DeductionReadDto { Id = 1 });
        var controller = new DeductionsController(mock.Object);

        var result = await controller.GetDeduction(1);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetDeduction_NonExisting_ReturnsNotFound()
    {
        var mock = new Mock<IDeductionService>();
        mock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((DeductionReadDto?)null);
        var controller = new DeductionsController(mock.Object);

        var result = await controller.GetDeduction(999);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateDeduction_Success_ReturnsCreated()
    {
        var dto = new DeductionCreateDto();
        var mock = new Mock<IDeductionService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<DeductionReadDto>.Success(new DeductionReadDto { Id = 1 }));
        var controller = new DeductionsController(mock.Object);

        var result = await controller.CreateDeduction(dto);

        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task CreateDeduction_Failure_ReturnsBadRequest()
    {
        var dto = new DeductionCreateDto();
        var mock = new Mock<IDeductionService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<DeductionReadDto>.Failure("error"));
        var controller = new DeductionsController(mock.Object);

        var result = await controller.CreateDeduction(dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateDeduction_Success_ReturnsNoContent()
    {
        var dto = new DeductionUpdateDto();
        var mock = new Mock<IDeductionService>();
        mock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(ServiceResult.Success());
        var controller = new DeductionsController(mock.Object);

        var result = await controller.UpdateDeduction(1, dto);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateDeduction_NotFound_ReturnsNotFound()
    {
        var dto = new DeductionUpdateDto();
        var mock = new Mock<IDeductionService>();
        mock.Setup(s => s.UpdateAsync(999, dto)).ReturnsAsync(ServiceResult.Missing());
        var controller = new DeductionsController(mock.Object);

        var result = await controller.UpdateDeduction(999, dto);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteDeduction_Success_ReturnsNoContent()
    {
        var mock = new Mock<IDeductionService>();
        mock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(ServiceResult.Success());
        var controller = new DeductionsController(mock.Object);

        var result = await controller.DeleteDeduction(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteDeduction_NotFound_ReturnsNotFound()
    {
        var mock = new Mock<IDeductionService>();
        mock.Setup(s => s.DeleteAsync(999)).ReturnsAsync(ServiceResult.Missing());
        var controller = new DeductionsController(mock.Object);

        var result = await controller.DeleteDeduction(999);

        result.Should().BeOfType<NotFoundResult>();
    }
}
