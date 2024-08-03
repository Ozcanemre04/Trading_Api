using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using trading_app.data;
using trading_app.dto.Wire;
using trading_app.interfaces;
using trading_app.models;
using trading_app.Validator.Wire;

namespace trading_app.services
{
    public class WireService : IWireService
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public WireService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor,
         UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _dbcontext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<WireDto> CreateWire(AddWireDto addWireDto)
        {
            string UserId = GetUserId();
            var user = await _userManager.FindByIdAsync(UserId);
            var SumOfWire = await GetSumOfWire();
            var tradePrice = await _dbcontext.Trades.Where(x => x.UserId == UserId)
                                                    .Select(trade => (trade.Close_price - trade.Open_price) * trade.Quantity)
                                                    .SumAsync();

            var OpenPnl = await _dbcontext.Trades.Where(x => x.UserId == UserId && x.Open == true)
                                                 .Select(trade => trade.Open_price * trade.Quantity)
                                                 .SumAsync();
            var sum = SumOfWire + tradePrice - OpenPnl + addWireDto.Amount;
            Log.Information("{a}", sum);
            if (SumOfWire + tradePrice - OpenPnl + addWireDto.Amount < 0.00M && addWireDto.Withdrawal == true)
            {
                throw new Exception("not enough money ");
            }

            var wire = _mapper.Map<Wire>(addWireDto);
            wire.UserId = UserId;
            wire.applicationUser = user!;

            _dbcontext.Wires.Add(wire);
            await _dbcontext.SaveChangesAsync();
            var WireDto = _mapper.Map<WireDto>(wire);
            var validator = new WireValidator();
            var validationResult = await validator.ValidateAsync(addWireDto);
            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
                throw new Exception(string.Join(Environment.NewLine, errorMessage));
            }
            return WireDto;
        }

        public async Task<decimal> CurrentBalance()
        {
            string UserId = GetUserId();
            var SumOfWire = await GetSumOfWire();
            var trade = await _dbcontext.Trades.Where(x => x.UserId == UserId && x.Open == false).Select(trade => (trade.Close_price!.Value - trade.Open_price) * trade.Quantity).SumAsync();
            Log.Information("{trade}", trade);
            var balance = SumOfWire + trade;

            return balance;
        }

        private string GetUserId()
        {
            return _httpContextAccessor!.HttpContext!.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        }
        private async Task<decimal> GetSumOfWire()
        {
            string userId = GetUserId();
            return await _dbcontext.Wires.Where(x => x.UserId == userId).SumAsync(x => x.Amount);

        }
    }
}