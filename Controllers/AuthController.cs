using Microsoft.AspNetCore.Mvc;
using MiniLearningPlatform.Api.DTOs;
using MiniLearningPlatform.Api.Services;

namespace MiniLearningPlatform.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new ApiErrorResponse("Username and password are required."));
        }

        var result = await _authService.LoginAsync(request);

        if (result is null)
        {
            // Deliberately generic message: do not reveal whether the username exists.
            return Unauthorized(new ApiErrorResponse("Invalid username or password."));
        }

        return Ok(result);
    }
}