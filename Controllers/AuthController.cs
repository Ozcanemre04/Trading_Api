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

        
    }

}