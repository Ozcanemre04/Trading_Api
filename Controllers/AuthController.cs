using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trading_app.dto.email;
using trading_app.dto.User;
using trading_app.interfaces.IServices;
using trading_app.Validator.User;

namespace trading_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterDto registerDto)
        {

            var response = await _authServices.Register(registerDto);
            return Ok(response);

        }
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authServices.Login(loginDto);
            return Ok(response);
        }

        [HttpPost("refreshToken")]
        [Authorize]
        public async Task<ActionResult<LoginResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var response = await _authServices.RefreshToken(refreshTokenDto);
            return Ok(response);
        }

        [HttpPut("confirm-email")]
        public async Task<ActionResult<ConfirmEmailResponse>> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
        {
            return Ok(await _authServices.ConfirmEmail(confirmEmailDto));
        }

        [HttpPost("resend-email-confirmation-link/{email}")]
        public async Task<ActionResult<ConfirmEmailResponse>> ResendEmailConfirmationLink([FromRoute] string email)
        {
            return Ok(await _authServices.ResendEmailConfirmationLink(email));
        }


        [HttpPost("forgot-username-or-password")]
        public async Task<ActionResult<ConfirmEmailResponse>> ForgotUsernameOrPassword(ForgotPasswordDto forgotPasswordDto)
        {
            return Ok(await _authServices.ForgotUsernameOrPassword(forgotPasswordDto));
        }

        [HttpPut("reset-password")]
        public async Task<ActionResult<ConfirmEmailResponse>> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            return Ok(await _authServices.ResetPassword(resetPasswordDto));
        }




    }

}