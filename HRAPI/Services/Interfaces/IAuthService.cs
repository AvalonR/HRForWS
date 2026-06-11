using HRAPI.DTOs.Auth;

namespace HRAPI.Services.Interfaces;

// Contract for authentication operations used by AuthController.
public interface IAuthService
{
    Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
    Task<CurrentUserResponse?> GetCurrentUserAsync(string email);
}
