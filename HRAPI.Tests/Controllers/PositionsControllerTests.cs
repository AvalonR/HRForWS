using HRAPI.Controllers;
using HRAPI.DTOs.Positions;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HRAPI.Tests.Controllers;

public class PositionsControllerTests
{
    [Fact]
    public async Task GetPositions_ReturnsOk()
    {
        var mock = new Mock<IPositionService>();
        mock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PositionReadDto>());
        var controller = new PositionsController(mock.Object);

        var result = await controller.GetPositions();

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPosition_Existing_ReturnsOk()
    {
        var mock = new Mock<IPositionService>();
        mock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new PositionReadDto { Id = 1 });
        var controller = new PositionsController(mock.Object);

        var result = await controller.GetPosition(1);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPosition_NonExisting_ReturnsNotFound()
    {
        var mock = new Mock<IPositionService>();
        mock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((PositionReadDto?)null);
        var controller = new PositionsController(mock.Object);

        var result = await controller.GetPosition(999);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreatePosition_Success_ReturnsCreated()
    {
        var dto = new PositionCreateDto();
        var mock = new Mock<IPositionService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<PositionReadDto>.Success(new PositionReadDto { Id = 1 }));
        var controller = new PositionsController(mock.Object);

        var result = await controller.CreatePosition(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task CreatePosition_Failure_ReturnsBadRequest()
    {
        var dto = new PositionCreateDto();
        var mock = new Mock<IPositionService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<PositionReadDto>.Failure("error"));
        var controller = new PositionsController(mock.Object);

        var result = await controller.CreatePosition(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdatePosition_Success_ReturnsNoContent()
    {
        var dto = new PositionUpdateDto();
        var mock = new Mock<IPositionService>();
        mock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(ServiceResult.Success());
        var controller = new PositionsController(mock.Object);

        var result = await controller.UpdatePosition(1, dto);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdatePosition_NotFound_ReturnsNotFound()
    {
        var dto = new PositionUpdateDto();
        var mock = new Mock<IPositionService>();
        mock.Setup(s => s.UpdateAsync(999, dto)).ReturnsAsync(ServiceResult.Missing());
        var controller = new PositionsController(mock.Object);

        var result = await controller.UpdatePosition(999, dto);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeletePosition_Success_ReturnsNoContent()
    {
        var mock = new Mock<IPositionService>();
        mock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(ServiceResult.Success());
        var controller = new PositionsController(mock.Object);

        var result = await controller.DeletePosition(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeletePosition_NotFound_ReturnsNotFound()
    {
        var mock = new Mock<IPositionService>();
        mock.Setup(s => s.DeleteAsync(999)).ReturnsAsync(ServiceResult.Missing());
        var controller = new PositionsController(mock.Object);

        var result = await controller.DeletePosition(999);

        result.Should().BeOfType<NotFoundResult>();
    }
}
