
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using trading_app.data;
using trading_app.dto.email;
using trading_app.dto.User;
using trading_app.exceptions;
using trading_app.interfaces;
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
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager,
        IConfiguration config, IHttpContextAccessor httpContextAccessor, IMapper mapper, ITokenService tokenService, IEmailService emailService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _mapper = mapper;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        //register
        public async Task<RegisterResponseDto> Register(RegisterDto registerDto)
        {
            var userAlreadyExist = await _userManager.FindByEmailAsync(registerDto.Email!);
            var UsernameTaken = await _userManager.FindByNameAsync(registerDto.Username!);
            var validator = new RegisterValidator();
            var validationResult = await validator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
                throw new BadRequestException(string.Join("|", errorMessage));
            }

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

            bool sendEmail = await SendConfirmEMailAsync(identityUser);
            if (!sendEmail) throw new BadRequestException("Failed to send email");

            var registerResponseDto = _mapper.Map<RegisterResponseDto>(identityUser);

            registerResponseDto.IsSucceed = true;
            registerResponseDto.Message = "Success: user is created, please confirm you email adress";
            return registerResponseDto;
        }
        //login
        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {
            var response = new LoginResponseDto();
            var Identity = await _userManager.FindByEmailAsync(loginDto.Email!) ?? throw new NotFoundException("user doesn't exist");
            if (Identity.EmailConfirmed == false)
            {
                throw new UnauthorizedException("please confirm your email before");
            }
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

            string token = _tokenService.GenerateToken(Identity);
            response.Message = "Success";
            response.AccessToken = token;
            response.refreshToken = _tokenService.GenerateRefreshToken();

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


            var principal = _tokenService.GetTokenPrincipal(refreshTokenDto.AccessToken);
            var response = new LoginResponseDto();

            if (principal?.Identity?.Name is null)
            {
                throw new BadRequestException("identity is null");
            }
            if (Identity is null || Identity.Refreshtoken != refreshTokenDto.refreshToken || Identity.RefreshTokenExpireTime < DateTime.Now)
            {
                throw new BadRequestException("wrong refresh token or refreshToken expired");
            }
            response.Message = "Success";
            response.AccessToken = _tokenService.GenerateToken(Identity);
            response.refreshToken = _tokenService.GenerateRefreshToken();

            Identity.Refreshtoken = response.refreshToken;
            Identity.RefreshTokenExpireTime = DateTime.Now.AddHours(12);
            await _userManager.UpdateAsync(Identity);

            return response;
        }
        //confirm email
        public async Task<ConfirmEmailResponse> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
        {
            var user = await _userManager.FindByEmailAsync(confirmEmailDto.Email) ?? throw new UnauthorizedException("This email address has not been registered yet");


            if (user.EmailConfirmed == true) throw new BadRequestException("Your email was confirmed before. Please login to your account");

            var result = await _userManager.ConfirmEmailAsync(user, confirmEmailDto.Token);
            if (!result.Succeeded)
            {
                throw new BadRequestException("Invalid token. Please try again");
            }
            var confirmEmailResponse = new ConfirmEmailResponse
            {
                Title = "Email confirmed",
                Message = "Your email address is confirmed. You can login now",
            };
            return confirmEmailResponse;
        }

        public async Task<ConfirmEmailResponse> ResendEmailConfirmationLink(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new BadRequestException("invalid email");
            var Identity = await _userManager.FindByEmailAsync(email) ?? throw new UnauthorizedException("This email address has not been registerd yet");
            if (Identity.EmailConfirmed == true) throw new UnauthorizedException("Your email address was confirmed before. Please login to your account");
            bool sendEmail = await SendConfirmEMailAsync(Identity);
            if (!sendEmail) throw new BadRequestException("Failed to send email");
            return new ConfirmEmailResponse
            {
                Title = "Confirmation link sent",
                Message = "Please confrim your email address",
            };
        }
        //forgetpassword
        public async Task<ConfirmEmailResponse> ForgotUsernameOrPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email) ?? throw new UnauthorizedException("This email address has not been registerd yet");
            if (user.EmailConfirmed == false) throw new BadRequestException("you should confirm your email adress first");
            bool Sendmail = await SendForgotUsernameOrPasswordEmail(user);
            if (!Sendmail) throw new BadRequestException("failed to send email");
            return new ConfirmEmailResponse
            {
                Title = "Forgot username or password email sent",
                Message = "Please check your email",
            };
        }

        public async Task<ConfirmEmailResponse> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var validator = new ResetPasswordValidator();
            var validationResult = await validator.ValidateAsync(resetPasswordDto);
            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
                throw new BadRequestException(string.Join("|", errorMessage));
            }
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email) ?? throw new UnauthorizedException("This email address has not been registered yet");
            if (user.EmailConfirmed == false) throw new BadRequestException("You should confirm your email adress first");
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!result.Succeeded) throw new BadRequestException("invalid token ,try again ");

            return new ConfirmEmailResponse
            {
                Title = "Password reset success",
                Message = "Your password has been reset",
            };

        }

        private async Task<bool> SendConfirmEMailAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = "front-end url";

            var body = $"<p>Hello: {user.FirstName} {user.LastName}</p>" +
                "<p>Please confirm your email address by clicking on the following link.</p>" +
                $"<p><a href=\"{url}\">Click here</a></p>" +
                "<p>copy and paste this token:</p>" +
                $"<em>{token}</em>" +
                "<p>Thank you,</p>" +
                $"<br>{_config["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(user.Email!, "Confirm your email", body);
            return await _emailService.SendEmailAsync(emailSend);
        }


        private async Task<bool> SendForgotUsernameOrPasswordEmail(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var TokenUrl = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"frontendUrl/{TokenUrl}";

            var body = $"<p>Hello: {user.FirstName} {user.LastName}</p>" +
               $"<p>Username: {user.UserName}.</p>" +
               "<p>In order to reset your password, please click on the following link.</p>" +
               $"<p><a href=\"{url}\">Click here</a></p>" +
               "<p>copy and paste this token:</p>" +
                $"<em>{token}</em>" +
               "<p>Thank you,</p>" +
               $"<br>{_config["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(user.Email!, "Forgot username or password", body);
            return await _emailService.SendEmailAsync(emailSend);
        }

    }
}