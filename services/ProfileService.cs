using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using trading_app.data;
using trading_app.dto.Profile;
using trading_app.exceptions;
using trading_app.interfaces;
using trading_app.models;
using trading_app.Validator.User;

namespace trading_app.services
{
    public class ProfileService : IProfileService
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IWireService _wireService;
        public ProfileService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor,
         UserManager<ApplicationUser> userManager, IMapper mapper, IWireService wireService)
        {
            _dbcontext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _mapper = mapper;
            _wireService = wireService;
        }
        public async Task<ProfileDto> Profile()
        {
            string UserId = _httpContextAccessor!.HttpContext!.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await _userManager.FindByIdAsync(UserId);
            var CurrentBalance = Getbalance(UserId);
            var profileDto = _mapper.Map<ProfileDto>(user);
            profileDto.CurrentBalance = await CurrentBalance;
            return profileDto;

        }

        public async Task<ProfileDto> UpdateProfile(ProfileUpdateDto profileUpdateDto)
        {
            string UserId = _httpContextAccessor!.HttpContext!.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await _userManager.FindByIdAsync(UserId);

            _mapper.Map(profileUpdateDto, user);
            await _userManager.UpdateAsync(user!);
            var updateProfileDto = _mapper.Map<ProfileDto>(user);
            updateProfileDto.CurrentBalance = await Getbalance(UserId);
            var validator = new ProfileValidator();
            var validationResult = await validator.ValidateAsync(profileUpdateDto);
            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
                throw new BadRequestException(string.Join("|", errorMessage));
            }
            return updateProfileDto;
        }

        private async Task<decimal> Getbalance(string id)
        {

            var wireSerivce = await _wireService.CurrentBalance();
            var OpenPnl = await _dbcontext.Trades.Where(x => x.UserId == id && x.Open == true)
                                                 .Select(trade => trade.Open_price * trade.Quantity)
                                                 .SumAsync();
            var CurrentBalance = wireSerivce - OpenPnl;
            return CurrentBalance;

        }
    }
}