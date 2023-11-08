using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trading_app.dto.User;

namespace trading_app.interfaces.IServices
{
    public interface IAuthServices
    {
        Task<RegisterResponseDto> Register(RegisterDto registerDto);

        Task<LoginResponseDto> Login(LoginDto loginDto);

        Task<string> RefreshToken();
        Task<string> Logout();
    }
}