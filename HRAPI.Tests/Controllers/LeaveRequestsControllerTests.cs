using HRAPI.Controllers;
using HRAPI.DTOs.LeaveRequests;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HRAPI.Tests.Controllers;

public class LeaveRequestsControllerTests
{
    [Fact]
    public async Task GetLeaveRequests_ReturnsOk()
    {
        var mock = new Mock<ILeaveRequestService>();
        mock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<LeaveRequestReadDto>());
        var controller = new LeaveRequestsController(mock.Object);

        var result = await controller.GetLeaveRequests();

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetLeaveRequest_Existing_ReturnsOk()
    {
        var mock = new Mock<ILeaveRequestService>();
        mock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new LeaveRequestReadDto { Id = 1 });
        var controller = new LeaveRequestsController(mock.Object);

        var result = await controller.GetLeaveRequest(1);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetLeaveRequest_NonExisting_ReturnsNotFound()
    {
        var mock = new Mock<ILeaveRequestService>();
        mock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((LeaveRequestReadDto?)null);
        var controller = new LeaveRequestsController(mock.Object);

        var result = await controller.GetLeaveRequest(999);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateLeaveRequest_Success_ReturnsCreated()
    {
        var dto = new LeaveRequestCreateDto();
        var mock = new Mock<ILeaveRequestService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<LeaveRequestReadDto>.Success(new LeaveRequestReadDto { Id = 1 }));
        var controller = new LeaveRequestsController(mock.Object);

        var result = await controller.CreateLeaveRequest(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task CreateLeaveRequest_Failure_ReturnsBadRequest()
    {
        var dto = new LeaveRequestCreateDto();
        var mock = new Mock<ILeaveRequestService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<LeaveRequestReadDto>.Failure("error"));
        var controller = new LeaveRequestsController(mock.Object);

        var result = await controller.CreateLeaveRequest(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateLeaveRequest_Success_ReturnsNoContent()
    {
        var dto = new LeaveRequestUpdateDto();
        var mock = new Mock<ILeaveRequestService>();
        mock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(ServiceResult.Success());
        var controller = new LeaveRequestsController(mock.Object);

        var result = await controller.UpdateLeaveRequest(1, dto);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateLeaveRequest_NotFound_ReturnsNotFound()
    {
        var dto = new LeaveRequestUpdateDto();
        var mock = new Mock<ILeaveRequestService>();
        mock.Setup(s => s.UpdateAsync(999, dto)).ReturnsAsync(ServiceResult.Missing());
        var controller = new LeaveRequestsController(mock.Object);

        var result = await controller.UpdateLeaveRequest(999, dto);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteLeaveRequest_Success_ReturnsNoContent()
    {
        var mock = new Mock<ILeaveRequestService>();
        mock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(ServiceResult.Success());
        var controller = new LeaveRequestsController(mock.Object);

        var result = await controller.DeleteLeaveRequest(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteLeaveRequest_NotFound_ReturnsNotFound()
    {
        var mock = new Mock<ILeaveRequestService>();
        mock.Setup(s => s.DeleteAsync(999)).ReturnsAsync(ServiceResult.Missing());
        var controller = new LeaveRequestsController(mock.Object);

        var result = await controller.DeleteLeaveRequest(999);

        result.Should().BeOfType<NotFoundResult>();
    }
}
