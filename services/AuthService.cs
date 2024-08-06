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
using trading_app.exceptions;
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
            var UsernameTaken = await _userManager.FindByNameAsync(registerDto.Username!);

            if (userAlreadyExist != null)
            {
                throw new BadRequestException("email already exist");
            }
            if (UsernameTaken != null)
            {
                throw new BadRequestException("UserName already taken ");
            }
            var identityUser = _mapper.Map<ApplicationUser>(registerDto);
            var result = await _userManager.CreateAsync(identityUser, registerDto.Password!);

            var validator = new RegisterValidator();
            var validationResult = await validator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
                throw new BadRequestException(string.Join("|", errorMessage));
            }
            // if (!result.Succeeded)
            // {
            //     var errorString = "";
            //     foreach (var error in result.Errors)
            //     {
            //         errorString += error.Description;
            //     }
            //     return new RegisterResponseDto()
            //     {
            //         IsSucceed = false,
            //         Message = errorString,
            //     };
            // }

            var registerResponseDto = _mapper.Map<RegisterResponseDto>(identityUser);

            registerResponseDto.IsSucceed = true;
            registerResponseDto.Message = "Success: user is created";

            return registerResponseDto;
        }
        //login
        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {
            var response = new LoginResponseDto();
            var Identity = await _userManager.FindByEmailAsync(loginDto.Email!) ?? throw new NotFoundException("user doesn't exist");


            var verifyPassword = await _userManager.CheckPasswordAsync(Identity, loginDto.Password!);
            if (!verifyPassword)
            {
                throw new BadRequestException("wrong password");
            };

            var validator = new LoginValidator();
            var validationResult = await validator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
                throw new BadRequestException(string.Join("|", errorMessage));
            }


            string token = this.GenerateToken(Identity);
            response.Message = "Success";
            response.AccessToken = token;
            response.refreshToken = this.GenerateRefreshToken();

            Identity.Refreshtoken = response.refreshToken;
            Identity.RefreshTokenExpireTime = DateTime.Now.AddHours(12);
            await _userManager.UpdateAsync(Identity);
            return response;
        }




        //refreshToken
        public async Task<LoginResponseDto> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var userid = _httpContextAccessor!.HttpContext!.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var Identity = await _userManager.FindByIdAsync(userid) ?? throw new NotFoundException("user doesn't exist");


            var principal = GetTokenPrincipal(refreshTokenDto.AccessToken);
            var response = new LoginResponseDto();
            if (principal.Identity.Name is null)
            {
                throw new BadRequestException("identity is null");
            }
            if (Identity is null || Identity.Refreshtoken != refreshTokenDto.refreshToken || Identity.RefreshTokenExpireTime < DateTime.Now)
            {
                throw new BadRequestException("wrong refresh token or refreshToken expired");
            }
            response.Message = "Success";
            response.AccessToken = this.GenerateToken(Identity);
            response.refreshToken = this.GenerateRefreshToken();

            Identity.Refreshtoken = response.refreshToken;
            Identity.RefreshTokenExpireTime = DateTime.Now.AddHours(12);
            await _userManager.UpdateAsync(Identity);

            return response;


        }

        private string GenerateRefreshToken()
        {
            var randonNumber = new Byte[64];
            using (var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randonNumber);
            }
            return Convert.ToBase64String(randonNumber);
        }
        private ClaimsPrincipal GetTokenPrincipal(string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JWT:Key").Value));
            var validation = new TokenValidationParameters
            {
                IssuerSigningKey = securityKey,
                ValidateLifetime = false,
                ValidateActor = false,
                ValidateIssuer = false,
                ValidateAudience = false,
            };
            return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
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


    }
}