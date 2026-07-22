using MiniLearningPlatform.Api.DTOs;

namespace MiniLearningPlatform.Api.Services;

public interface IAuthService
{
    // Returns null when credentials are invalid so the controller can return a 401.
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}