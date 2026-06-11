using HRAPI.Controllers;
using HRAPI.DTOs.Departments;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HRAPI.Tests.Controllers;

public class DepartmentsControllerTests
{
    [Fact]
    public async Task GetDepartments_ReturnsOk()
    {
        var mock = new Mock<IDepartmentService>();
        mock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<DepartmentReadDto>());
        var controller = new DepartmentsController(mock.Object);

        var result = await controller.GetDepartments();

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetDepartment_Existing_ReturnsOk()
    {
        var mock = new Mock<IDepartmentService>();
        mock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new DepartmentReadDto { Id = 1 });
        var controller = new DepartmentsController(mock.Object);

        var result = await controller.GetDepartment(1);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetDepartment_NonExisting_ReturnsNotFound()
    {
        var mock = new Mock<IDepartmentService>();
        mock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((DepartmentReadDto?)null);
        var controller = new DepartmentsController(mock.Object);

        var result = await controller.GetDepartment(999);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateDepartment_Success_ReturnsCreated()
    {
        var dto = new DepartmentCreateDto();
        var mock = new Mock<IDepartmentService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<DepartmentReadDto>.Success(new DepartmentReadDto { Id = 1 }));
        var controller = new DepartmentsController(mock.Object);

        var result = await controller.CreateDepartment(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task CreateDepartment_Failure_ReturnsBadRequest()
    {
        var dto = new DepartmentCreateDto();
        var mock = new Mock<IDepartmentService>();
        mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<DepartmentReadDto>.Failure("error"));
        var controller = new DepartmentsController(mock.Object);

        var result = await controller.CreateDepartment(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateDepartment_Success_ReturnsNoContent()
    {
        var dto = new DepartmentUpdateDto();
        var mock = new Mock<IDepartmentService>();
        mock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(ServiceResult.Success());
        var controller = new DepartmentsController(mock.Object);

        var result = await controller.UpdateDepartment(1, dto);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateDepartment_NotFound_ReturnsNotFound()
    {
        var dto = new DepartmentUpdateDto();
        var mock = new Mock<IDepartmentService>();
        mock.Setup(s => s.UpdateAsync(999, dto)).ReturnsAsync(ServiceResult.Missing());
        var controller = new DepartmentsController(mock.Object);

        var result = await controller.UpdateDepartment(999, dto);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteDepartment_Success_ReturnsNoContent()
    {
        var mock = new Mock<IDepartmentService>();
        mock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(ServiceResult.Success());
        var controller = new DepartmentsController(mock.Object);

        var result = await controller.DeleteDepartment(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteDepartment_NotFound_ReturnsNotFound()
    {
        var mock = new Mock<IDepartmentService>();
        mock.Setup(s => s.DeleteAsync(999)).ReturnsAsync(ServiceResult.Missing());
        var controller = new DepartmentsController(mock.Object);

        var result = await controller.DeleteDepartment(999);

        result.Should().BeOfType<NotFoundResult>();
    }
}
