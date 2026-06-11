using HRAPI.Controllers;
using HRAPI.DTOs.Auth;
using HRAPI.Services;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HRAPI.Tests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_CredentialsValid_ReturnsOk()
    {
        var dto = new LoginRequest();
        var mock = new Mock<IAuthService>();
        mock.Setup(s => s.LoginAsync(dto)).ReturnsAsync(
            ServiceResult<LoginResponse>.Success(new LoginResponse()));
        var controller = new AuthController(mock.Object);

        var result = await controller.Login(dto);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Login_CredentialsInvalid_ReturnsUnauthorized()
    {
        var dto = new LoginRequest();
        var mock = new Mock<IAuthService>();
        mock.Setup(s => s.LoginAsync(dto)).ReturnsAsync(
            ServiceResult<LoginResponse>.Failure("Invalid credentials"));
        var controller = new AuthController(mock.Object);

        var result = await controller.Login(dto);

        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetCurrentUser_Authenticated_ReturnsOk()
    {
        var mock = new Mock<IAuthService>();
        mock.Setup(s => s.GetCurrentUserAsync("user@test.com"))
            .ReturnsAsync(new CurrentUserResponse());
        var controller = new AuthController(mock.Object);
        controller.SetupUser("user@test.com");

        var result = await controller.GetCurrentUser();

        result.Result.Should().BeOfType<OkObjectResult>();
    }
}
