using HRAPI.Controllers;
using HRAPI.DTOs.SalaryHistories;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HRAPI.Tests.Controllers;

public class SalaryHistoriesControllerTests
{
    [Fact]
    public async Task GetSalaryHistories_ReturnsOk()
    {
        var mock = new Mock<ISalaryHistoryService>();
        mock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<SalaryHistoryReadDto>());
        var controller = new SalaryHistoriesController(mock.Object);

        var result = await controller.GetSalaryHistories();

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetSalaryHistory_Existing_ReturnsOk()
    {
        var mock = new Mock<ISalaryHistoryService>();
        mock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new SalaryHistoryReadDto { Id = 1 });
        var controller = new SalaryHistoriesController(mock.Object);

        var result = await controller.GetSalaryHistory(1);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetSalaryHistory_NonExisting_ReturnsNotFound()
    {
        var mock = new Mock<ISalaryHistoryService>();
        mock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((SalaryHistoryReadDto?)null);
        var controller = new SalaryHistoriesController(mock.Object);

        var result = await controller.GetSalaryHistory(999);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateSalaryHistory_Success_ReturnsCreated()
    {
        var dto = new SalaryHistoryCreateDto();
        var mock = new Mock<ISalaryHistoryService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<SalaryHistoryReadDto>.Success(new SalaryHistoryReadDto { Id = 1 }));
        var controller = new SalaryHistoriesController(mock.Object);

        var result = await controller.CreateSalaryHistory(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task CreateSalaryHistory_Failure_ReturnsBadRequest()
    {
        var dto = new SalaryHistoryCreateDto();
        var mock = new Mock<ISalaryHistoryService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<SalaryHistoryReadDto>.Failure("error"));
        var controller = new SalaryHistoriesController(mock.Object);

        var result = await controller.CreateSalaryHistory(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateSalaryHistory_Success_ReturnsNoContent()
    {
        var dto = new SalaryHistoryUpdateDto();
        var mock = new Mock<ISalaryHistoryService>();
        mock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(ServiceResult.Success());
        var controller = new SalaryHistoriesController(mock.Object);

        var result = await controller.UpdateSalaryHistory(1, dto);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateSalaryHistory_NotFound_ReturnsNotFound()
    {
        var dto = new SalaryHistoryUpdateDto();
        var mock = new Mock<ISalaryHistoryService>();
        mock.Setup(s => s.UpdateAsync(999, dto)).ReturnsAsync(ServiceResult.Missing());
        var controller = new SalaryHistoriesController(mock.Object);

        var result = await controller.UpdateSalaryHistory(999, dto);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteSalaryHistory_Success_ReturnsNoContent()
    {
        var mock = new Mock<ISalaryHistoryService>();
        mock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(ServiceResult.Success());
        var controller = new SalaryHistoriesController(mock.Object);

        var result = await controller.DeleteSalaryHistory(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteSalaryHistory_NotFound_ReturnsNotFound()
    {
        var mock = new Mock<ISalaryHistoryService>();
        mock.Setup(s => s.DeleteAsync(999)).ReturnsAsync(ServiceResult.Missing());
        var controller = new SalaryHistoriesController(mock.Object);

        var result = await controller.DeleteSalaryHistory(999);

        result.Should().BeOfType<NotFoundResult>();
    }
}
