using HRAPI.DTOs.Auth;

namespace HRAPI.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
    Task<CurrentUserResponse?> GetCurrentUserAsync(string email);
}
