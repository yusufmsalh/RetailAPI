using RetailAPI.Core.Entities;

namespace RetailAPI.Core.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    DateTime GetExpiration();
}
