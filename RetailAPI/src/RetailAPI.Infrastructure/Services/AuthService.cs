using Microsoft.Extensions.Logging;
using RetailAPI.Core.DTOs;
using RetailAPI.Core.Exceptions;
using RetailAPI.Core.Interfaces;

namespace RetailAPI.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService, ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
            throw new UnauthorizedException("Invalid username or password.");
        }

        var token = _jwtTokenService.GenerateToken(user);
        var expiresAt = _jwtTokenService.GetExpiration();

        _logger.LogInformation("User {Username} logged in successfully.", user.Username);

        return new LoginResponse(token, user.Username, user.Role, expiresAt);
    }
}
