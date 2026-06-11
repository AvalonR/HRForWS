using HRAPI.Controllers;
using HRAPI.DTOs.Attendances;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HRAPI.Tests.Controllers;

public class AttendancesControllerTests
{
    [Fact]
    public async Task GetAttendances_ReturnsOk()
    {
        var mock = new Mock<IAttendanceService>();
        mock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<AttendanceReadDto>());
        var controller = new AttendancesController(mock.Object);

        var result = await controller.GetAttendances();

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetAttendance_Existing_ReturnsOk()
    {
        var mock = new Mock<IAttendanceService>();
        mock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new AttendanceReadDto { Id = 1 });
        var controller = new AttendancesController(mock.Object);

        var result = await controller.GetAttendance(1);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetAttendance_NonExisting_ReturnsNotFound()
    {
        var mock = new Mock<IAttendanceService>();
        mock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((AttendanceReadDto?)null);
        var controller = new AttendancesController(mock.Object);

        var result = await controller.GetAttendance(999);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateAttendance_Success_ReturnsCreated()
    {
        var dto = new AttendanceCreateDto();
        var mock = new Mock<IAttendanceService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<AttendanceReadDto>.Success(new AttendanceReadDto { Id = 1 }));
        var controller = new AttendancesController(mock.Object);

        var result = await controller.CreateAttendance(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task CreateAttendance_Failure_ReturnsBadRequest()
    {
        var dto = new AttendanceCreateDto();
        var mock = new Mock<IAttendanceService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<AttendanceReadDto>.Failure("error"));
        var controller = new AttendancesController(mock.Object);

        var result = await controller.CreateAttendance(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateAttendance_Success_ReturnsNoContent()
    {
        var dto = new AttendanceUpdateDto();
        var mock = new Mock<IAttendanceService>();
        mock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(ServiceResult.Success());
        var controller = new AttendancesController(mock.Object);

        var result = await controller.UpdateAttendance(1, dto);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateAttendance_NotFound_ReturnsNotFound()
    {
        var dto = new AttendanceUpdateDto();
        var mock = new Mock<IAttendanceService>();
        mock.Setup(s => s.UpdateAsync(999, dto)).ReturnsAsync(ServiceResult.Missing());
        var controller = new AttendancesController(mock.Object);

        var result = await controller.UpdateAttendance(999, dto);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAttendance_Success_ReturnsNoContent()
    {
        var mock = new Mock<IAttendanceService>();
        mock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(ServiceResult.Success());
        var controller = new AttendancesController(mock.Object);

        var result = await controller.DeleteAttendance(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteAttendance_NotFound_ReturnsNotFound()
    {
        var mock = new Mock<IAttendanceService>();
        mock.Setup(s => s.DeleteAsync(999)).ReturnsAsync(ServiceResult.Missing());
        var controller = new AttendancesController(mock.Object);

        var result = await controller.DeleteAttendance(999);

        result.Should().BeOfType<NotFoundResult>();
    }
}
