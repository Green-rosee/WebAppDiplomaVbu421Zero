using WebAppEstimate.Data.Entity;

namespace WebAppEstimate.Services;

public interface IJwtTokenService
{
    string CreateToken(UserAuthz user);
}