using HRAPI.Controllers;
using HRAPI.DTOs.PerformanceReviews;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HRAPI.Tests.Controllers;

public class PerformanceReviewsControllerTests
{
    [Fact]
    public async Task GetPerformanceReviews_ReturnsOk()
    {
        var mock = new Mock<IPerformanceReviewService>();
        mock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PerformanceReviewReadDto>());
        var controller = new PerformanceReviewsController(mock.Object);

        var result = await controller.GetPerformanceReviews();

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPerformanceReview_Existing_ReturnsOk()
    {
        var mock = new Mock<IPerformanceReviewService>();
        mock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new PerformanceReviewReadDto { Id = 1 });
        var controller = new PerformanceReviewsController(mock.Object);

        var result = await controller.GetPerformanceReview(1);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPerformanceReview_NonExisting_ReturnsNotFound()
    {
        var mock = new Mock<IPerformanceReviewService>();
        mock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((PerformanceReviewReadDto?)null);
        var controller = new PerformanceReviewsController(mock.Object);

        var result = await controller.GetPerformanceReview(999);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreatePerformanceReview_Success_ReturnsCreated()
    {
        var dto = new PerformanceReviewCreateDto();
        var mock = new Mock<IPerformanceReviewService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<PerformanceReviewReadDto>.Success(new PerformanceReviewReadDto { Id = 1 }));
        var controller = new PerformanceReviewsController(mock.Object);

        var result = await controller.CreatePerformanceReview(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task CreatePerformanceReview_Failure_ReturnsBadRequest()
    {
        var dto = new PerformanceReviewCreateDto();
        var mock = new Mock<IPerformanceReviewService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<PerformanceReviewReadDto>.Failure("error"));
        var controller = new PerformanceReviewsController(mock.Object);

        var result = await controller.CreatePerformanceReview(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdatePerformanceReview_Success_ReturnsNoContent()
    {
        var dto = new PerformanceReviewUpdateDto();
        var mock = new Mock<IPerformanceReviewService>();
        mock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(ServiceResult.Success());
        var controller = new PerformanceReviewsController(mock.Object);

        var result = await controller.UpdatePerformanceReview(1, dto);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdatePerformanceReview_NotFound_ReturnsNotFound()
    {
        var dto = new PerformanceReviewUpdateDto();
        var mock = new Mock<IPerformanceReviewService>();
        mock.Setup(s => s.UpdateAsync(999, dto)).ReturnsAsync(ServiceResult.Missing());
        var controller = new PerformanceReviewsController(mock.Object);

        var result = await controller.UpdatePerformanceReview(999, dto);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeletePerformanceReview_Success_ReturnsNoContent()
    {
        var mock = new Mock<IPerformanceReviewService>();
        mock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(ServiceResult.Success());
        var controller = new PerformanceReviewsController(mock.Object);

        var result = await controller.DeletePerformanceReview(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeletePerformanceReview_NotFound_ReturnsNotFound()
    {
        var mock = new Mock<IPerformanceReviewService>();
        mock.Setup(s => s.DeleteAsync(999)).ReturnsAsync(ServiceResult.Missing());
        var controller = new PerformanceReviewsController(mock.Object);

        var result = await controller.DeletePerformanceReview(999);

        result.Should().BeOfType<NotFoundResult>();
    }
}
