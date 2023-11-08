using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using trading_app.data;
using trading_app.dto.User;
using trading_app.interfaces.IServices;
using trading_app.models;
using trading_app.Validator.User;

namespace trading_app.services
{
    public class AuthService : IAuthServices
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public AuthService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager,
        IConfiguration config, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _mapper = mapper;
        }

        //register
        public async Task<RegisterResponseDto> Register(RegisterDto registerDto)
        {
            var userAlreadyExist = await _userManager.FindByEmailAsync(registerDto.Email!);

            if (userAlreadyExist != null)
            {
                throw new Exception("email already exist");
            }
            var identityUser = _mapper.Map<ApplicationUser>(registerDto);
            var result = await _userManager.CreateAsync(identityUser, registerDto.Password!);
            var validator = new RegisterValidator();
            var validationResult = await validator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
                throw new Exception(string.Join(Environment.NewLine, errorMessage));
            }
            if (!result.Succeeded)
            {
                var errorString = "";
                foreach (var error in result.Errors)
                {
                    errorString += error.Description;
                }
                return new RegisterResponseDto()
                {
                    IsSucceed = false,
                    Message = errorString,
                };
            }

            var registerResponseDto = _mapper.Map<RegisterResponseDto>(identityUser);

            registerResponseDto.IsSucceed = true;
            registerResponseDto.Message = "Success: user is created";

            return registerResponseDto;
        }
        //login
        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {
            var Identity = await _userManager.FindByEmailAsync(loginDto.Email!) ?? throw new Exception("user doesn't exist");
            var refresh = await _dbContext.RefreshToken.FirstOrDefaultAsync(x => x.UserId == Identity!.Id);

            var verifyPassword = await _userManager.CheckPasswordAsync(Identity, loginDto.Password!);
            if (!verifyPassword)
            {
                throw new Exception("wrong password");
            };
            if (Identity.Refreshtoken != null)
            {
                var token0 = GenerateToken(Identity);
                refresh!.Refreshtoken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(100));
                refresh.Expires = DateTime.Now.AddDays(7);
                await _dbContext.SaveChangesAsync();
                SetRefreshToken(refresh);
                return new LoginResponseDto()
                {
                    Message = "Success",
                    AccessToken = token0,
                };
            };
            var validator = new LoginValidator();
            var validationResult = await validator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
                throw new Exception(string.Join(Environment.NewLine, errorMessage));
            }
            var refreshToken = new RefreshToken
            {
                Refreshtoken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(100)),
                Expires = DateTime.Now.AddDays(7),
                UserId = Identity.Id,
                applicationUser = Identity
            };

            _dbContext.RefreshToken.Add(refreshToken);
            await _dbContext.SaveChangesAsync();
            SetRefreshToken(refreshToken);

            string token = GenerateToken(Identity);
            return new LoginResponseDto()
            {
                Message = "Success",
                AccessToken = token
            };

        }


        //logout
        public async Task<string> Logout()
        {
            var userid = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var user = await _userManager.FindByIdAsync(userid);
            var refresh = await _dbContext.RefreshToken.FirstOrDefaultAsync(x => x.UserId == userid);

            if (refresh != null)
            {
                _dbContext.RefreshToken.Remove(refresh);
                await _dbContext.SaveChangesAsync();
                _httpContextAccessor?.HttpContext?.Response.Cookies.Delete("refreshToken");
                var msg = "cookie deleted";
                return msg;
            }
            else
            {
                var errorMessage = "cookie not found";
                return errorMessage;
            }
        }


        //refreshToken
        public async Task<string> RefreshToken()
        {
            var GetRefreshtoken = _httpContextAccessor!.HttpContext!.Request.Cookies["refreshToken"];
            var userid = _httpContextAccessor!.HttpContext!.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await _userManager.FindByIdAsync(userid);
            var refresh = await _dbContext.RefreshToken.FirstOrDefaultAsync(x => x.UserId == userid);
            if (refresh?.Refreshtoken == null)
            {
                string msg = "refresh token is null";
                return msg;
            }
            if (!refresh.Refreshtoken.Equals(GetRefreshtoken))
            {
                string msg = "Invalid refresh Token";
                return msg;
            }
            else if (refresh?.Expires < DateTime.Now)
            {
                string msg = "Token expired";
                return msg;
            }
            string token = GenerateToken(user!);
            refresh!.Refreshtoken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(100));
            refresh.Expires = DateTime.Now.AddDays(7);
            await _dbContext.SaveChangesAsync();
            SetRefreshToken(refresh);
            return token;

        }


        private string GenerateToken(ApplicationUser user)
        {

            var claims = new List<Claim>{
                new Claim(ClaimTypes.Email,user.Email ?? ""),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.UserName ?? ""),
            };

            string key = _config.GetSection("JWT:Key").Value ?? "";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(600),
                issuer: _config.GetSection("JWT:Issuer").Value,
                audience: _config.GetSection("JWT:Audience").Value,
                signingCredentials: signingCred);

            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }

        private void SetRefreshToken(RefreshToken newToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newToken.Expires,
            };
            _httpContextAccessor?.HttpContext?.Response.Cookies.Append("refreshToken", newToken.Refreshtoken, cookieOptions);

        }


    }
}