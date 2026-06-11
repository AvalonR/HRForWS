using HRAPI.Controllers;
using HRAPI.DTOs.Employees;
using HRAPI.Models;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HRAPI.Tests.Controllers;

public class EmployeesControllerTests
{
    [Fact]
    public async Task GetEmployees_ReturnsOk()
    {
        var mockService = new Mock<IEmployeeService>();
        mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<EmployeeReadDto>());
        var mockUserManager = UserManagerMock();
        var controller = new EmployeesController(mockService.Object, mockUserManager.Object);

        var result = await controller.GetEmployees();

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetEmployee_Existing_ReturnsOk()
    {
        var mockService = new Mock<IEmployeeService>();
        mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new EmployeeReadDto { Id = 1 });
        var mockUserManager = UserManagerMock();
        mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser { EmployeeId = null });
        var controller = new EmployeesController(mockService.Object, mockUserManager.Object);
        controller.SetupUser("admin@test.com", "Admin");

        var result = await controller.GetEmployee(1);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetEmployee_NonExisting_ReturnsNotFound()
    {
        var mockService = new Mock<IEmployeeService>();
        mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((EmployeeReadDto?)null);
        var mockUserManager = UserManagerMock();
        mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser { EmployeeId = null });
        var controller = new EmployeesController(mockService.Object, mockUserManager.Object);
        controller.SetupUser("admin@test.com", "Admin");

        var result = await controller.GetEmployee(999);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetEmployee_NonAdminDifferentUser_ReturnsForbid()
    {
        var mockService = new Mock<IEmployeeService>();
        mockService.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(new EmployeeReadDto { Id = 2 });
        var mockUserManager = UserManagerMock();
        mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser { EmployeeId = 1 });
        var controller = new EmployeesController(mockService.Object, mockUserManager.Object);
        controller.SetupUser("user@test.com", "Employee");

        var result = await controller.GetEmployee(2);

        result.Result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task CreateEmployee_Success_ReturnsCreated()
    {
        var dto = new EmployeeCreateDto();
        var mockService = new Mock<IEmployeeService>();
        mockService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<EmployeeReadDto>.Success(new EmployeeReadDto { Id = 1 }));
        var mockUserManager = UserManagerMock();
        var controller = new EmployeesController(mockService.Object, mockUserManager.Object);

        var result = await controller.CreateEmployee(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task UpdateEmployee_Success_ReturnsNoContent()
    {
        var dto = new EmployeeUpdateDto();
        var mockService = new Mock<IEmployeeService>();
        mockService.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(ServiceResult.Success());
        var mockUserManager = UserManagerMock();
        var controller = new EmployeesController(mockService.Object, mockUserManager.Object);

        var result = await controller.UpdateEmployee(1, dto);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteEmployee_Success_ReturnsNoContent()
    {
        var mockService = new Mock<IEmployeeService>();
        mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(ServiceResult.Success());
        var mockUserManager = UserManagerMock();
        var controller = new EmployeesController(mockService.Object, mockUserManager.Object);

        var result = await controller.DeleteEmployee(1);

        result.Should().BeOfType<NoContentResult>();
    }

    private static Mock<UserManager<AppUser>> UserManagerMock()
    {
        return new Mock<UserManager<AppUser>>(
            Mock.Of<IUserStore<AppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
    }
}
