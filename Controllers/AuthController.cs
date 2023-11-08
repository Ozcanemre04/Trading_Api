using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            try
            {

                var response = await _authServices.Register(registerDto);
                if (response.IsSucceed)
                {
                    return Ok(response);
                }
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {

                var response = await _authServices.Login(loginDto);

                return Ok(response);

            }
            catch (Exception ex)
            {
                if (ex.Message == "user doesn't exist")
                {
                    return NotFound(ex.Message);
                }
                if (ex.Message == "wrong password")
                {
                    return Unauthorized(ex.Message);
                }
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("refreshToken")]
        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var response = await _authServices.RefreshToken();
                if (response == "Invalid refresh Token" || response == "Token expired")
                {
                    return BadRequest(response);
                }
                else if (response == "Token")
                {
                    return Unauthorized(response);
                }
                return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var response = await _authServices.Logout();
                if (response == "cookie not found")
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }
    }

}