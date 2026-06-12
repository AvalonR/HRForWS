using HRAPI.Controllers;
using HRAPI.DTOs.LeaveTypes;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HRAPI.Tests.Controllers;

public class LeaveTypesControllerTests
{
    [Fact]
    public async Task GetLeaveTypes_ReturnsOk()
    {
        var mock = new Mock<ILeaveTypeService>();
        mock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<LeaveTypeReadDto>());
        var controller = new LeaveTypesController(mock.Object);

        var result = await controller.GetLeaveTypes();

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetLeaveType_Existing_ReturnsOk()
    {
        var mock = new Mock<ILeaveTypeService>();
        mock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new LeaveTypeReadDto { Id = 1 });
        var controller = new LeaveTypesController(mock.Object);

        var result = await controller.GetLeaveType(1);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetLeaveType_NonExisting_ReturnsNotFound()
    {
        var mock = new Mock<ILeaveTypeService>();
        mock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((LeaveTypeReadDto?)null);
        var controller = new LeaveTypesController(mock.Object);

        var result = await controller.GetLeaveType(999);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateLeaveType_Success_ReturnsCreated()
    {
        var dto = new LeaveTypeCreateDto();
        var mock = new Mock<ILeaveTypeService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<LeaveTypeReadDto>.Success(new LeaveTypeReadDto { Id = 1 }));
        var controller = new LeaveTypesController(mock.Object);

        var result = await controller.CreateLeaveType(dto);

        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task CreateLeaveType_Failure_ReturnsBadRequest()
    {
        var dto = new LeaveTypeCreateDto();
        var mock = new Mock<ILeaveTypeService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<LeaveTypeReadDto>.Failure("error"));
        var controller = new LeaveTypesController(mock.Object);

        var result = await controller.CreateLeaveType(dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateLeaveType_Success_ReturnsNoContent()
    {
        var dto = new LeaveTypeUpdateDto();
        var mock = new Mock<ILeaveTypeService>();
        mock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(ServiceResult.Success());
        var controller = new LeaveTypesController(mock.Object);

        var result = await controller.UpdateLeaveType(1, dto);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateLeaveType_NotFound_ReturnsNotFound()
    {
        var dto = new LeaveTypeUpdateDto();
        var mock = new Mock<ILeaveTypeService>();
        mock.Setup(s => s.UpdateAsync(999, dto)).ReturnsAsync(ServiceResult.Missing());
        var controller = new LeaveTypesController(mock.Object);

        var result = await controller.UpdateLeaveType(999, dto);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteLeaveType_Success_ReturnsNoContent()
    {
        var mock = new Mock<ILeaveTypeService>();
        mock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(ServiceResult.Success());
        var controller = new LeaveTypesController(mock.Object);

        var result = await controller.DeleteLeaveType(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteLeaveType_NotFound_ReturnsNotFound()
    {
        var mock = new Mock<ILeaveTypeService>();
        mock.Setup(s => s.DeleteAsync(999)).ReturnsAsync(ServiceResult.Missing());
        var controller = new LeaveTypesController(mock.Object);

        var result = await controller.DeleteLeaveType(999);

        result.Should().BeOfType<NotFoundResult>();
    }
}
