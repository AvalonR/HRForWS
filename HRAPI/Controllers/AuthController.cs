using HRAPI.DTOs.Auth;
using HRAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRAPI.Controllers;

[Route("api/[controller]")]
// Handles login and current-user endpoints for JWT-based authentication.
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        if (!result.Succeeded)
            return Unauthorized();

        return Ok(result.Data);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<CurrentUserResponse>> GetCurrentUser()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email == null) return Unauthorized();

        var user = await _authService.GetCurrentUserAsync(email);
        if (user == null) return Unauthorized();

        return Ok(user);
    }
}
