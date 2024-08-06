using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using trading_app.models;

namespace trading_app.interfaces
{
    public interface ITokenService
    {
        string GenerateRefreshToken();
        ClaimsPrincipal GetTokenPrincipal(string token);
        string GenerateToken(ApplicationUser user);
    }
}