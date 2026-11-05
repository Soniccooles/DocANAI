using DocANAI.Persistence.Entities.Users;

namespace DocANAI.Api.Infrastructure.Authentication;

public interface IJwtProvider
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}