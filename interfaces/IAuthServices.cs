using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trading_app.dto.email;
using trading_app.dto.User;

namespace trading_app.interfaces.IServices
{
    public interface IAuthServices
    {
        Task<RegisterResponseDto> Register(RegisterDto registerDto);

        Task<LoginResponseDto> Login(LoginDto loginDto);

        Task<LoginResponseDto> RefreshToken(RefreshTokenDto refreshTokenDto);

        Task<ConfirmEmailResponse> ConfirmEmail(ConfirmEmailDto confirmEmailDto);
        Task<ConfirmEmailResponse> ResendEmailConfirmationLink(string email);
        Task<ConfirmEmailResponse> ForgotUsernameOrPassword(ForgotPasswordDto forgotPasswordDto);
        Task<ConfirmEmailResponse> ResetPassword(ResetPasswordDto resetPasswordDto);
        
    }
}