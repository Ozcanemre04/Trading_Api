using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using trading_app.data;
using trading_app.dto.trade;
using trading_app.interfaces;
using trading_app.models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Newtonsoft.Json;
using trading_app.Validator.Trade;
using trading_app.exceptions;
using trading_app.dto;

namespace trading_app.services
{
    public class TradeService : ITradeService
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;


        public TradeService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor,
         UserManager<ApplicationUser> userManager, IMapper mapper, HttpClient httpClient)
        {
            _dbcontext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _mapper = mapper;
            _httpClient = httpClient;

        }

        public async Task<PageDto<TradeDto>> AllTrades(int pageNumber, int pageSize)
        {
            string userId = GetUserId();
            var count = await _dbcontext.Trades.Where(x => x.UserId == userId).CountAsync();
            var allTrades = await _dbcontext.Trades.Where(x => x.UserId == userId)
                                            .Skip((pageNumber-1) * pageSize)
                                            .Take(pageSize).ToListAsync();
            var tradesDto = allTrades.Select(trade => _mapper.Map<TradeDto>(trade));
            PageDto<TradeDto> pageDto = new  PageDto<TradeDto>{
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = count / pageSize,
                TotalRecords = count,
                Data = tradesDto
            };
            return pageDto;
        }

        public async Task<TradeDto> GetOneTrade(Guid id)
        {
            string userId = GetUserId();
            var Trade = await _dbcontext.Trades.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId) ?? throw new NotFoundException("trade is not found");
            var tradeDto = _mapper.Map<TradeDto>(Trade);
            return tradeDto;
        }


        public async Task<IEnumerable<TradeDto>> GetAllOpenTrades()
        {
            string userId = GetUserId();
            var allTrades = await _dbcontext.Trades.Where(x => x.UserId == userId && x.Open == true).ToListAsync();
            var tradesDto = allTrades.Select(trade => _mapper.Map<TradeDto>(trade));
            return tradesDto;
        }

        public async Task<IEnumerable<TradeDto>> GetAllClosedTrades()
        {
            string userId = GetUserId();
            var allTrades = await _dbcontext.Trades.Where(x => x.UserId == userId && x.Open == false).ToListAsync();
            var tradesDto = allTrades.Select(trade => _mapper.Map<TradeDto>(trade));
            return tradesDto;
        }

        public async Task<PnlDto> GetOpenpnl()
        {
            var OpenPnl = await GetPNL(true);
            decimal OpenPnll = OpenPnl.DefaultIfEmpty(0.00m).Sum();
            var openPnlDto = new PnlDto
            {
                Pnl = OpenPnll,
                Open = true
            };
            return openPnlDto;
        }
        public async Task<PnlDto> GetClosedpnl()
        {
            var ClosePnl = await GetPNL(false);
            decimal ClosePnll = ClosePnl.DefaultIfEmpty(0.00m).Sum();
            var ClosePnlDto = new PnlDto
            {
                Pnl = ClosePnll,
                Open = false
            };

            return ClosePnlDto;
        }

        public async Task<TradeDto> OpenTrade(AddTradeDto addTradeDto)
        {
            string UserId = GetUserId();
            var user = await _userManager.FindByIdAsync(UserId);
            var wire = await _dbcontext.Wires.Where(x => x.UserId == UserId).SumAsync(x => x.Amount);
            var tradePrice = await _dbcontext.Trades.Where(x => x.UserId == UserId).Select(trade => (trade.Close_price - trade.Open_price) * trade.Quantity).SumAsync();
            var data = await Get_realtime_price(addTradeDto.Symbol!);
            var OpenPnl = await _dbcontext.Trades.Where(x => x.UserId == UserId && x.Open == true)
                                                 .Select(trade => trade.Open_price * trade.Quantity)
                                                 .SumAsync();
            var validator = new TradeValidator();
            var validationResult = await validator.ValidateAsync(addTradeDto);
            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
                throw new BadRequestException(string.Join("|", errorMessage));
            }
            if (wire + tradePrice - OpenPnl - (data * addTradeDto.Quantity) < 0.00m)
            {
                throw new BadRequestException("not enough money");
            }

            var openTrade = _mapper.Map<Trade>(addTradeDto);
            openTrade.UserId = UserId;
            openTrade.applicationUser = user!;
            openTrade.Open_price = data;
            _dbcontext.Trades.Add(openTrade);
            await _dbcontext.SaveChangesAsync();
            var openTradeDto = _mapper.Map<TradeDto>(openTrade);
            return openTradeDto;
        }

        public async Task<TradeDto> CloseTrade(Guid id)
        {
            string UserId = GetUserId();
            var user = await _userManager.FindByIdAsync(UserId);
            var findTrade = await _dbcontext.Trades.FirstOrDefaultAsync(x => x.Id == id && x.UserId == UserId && x.Open == true) ?? throw new NotFoundException("not found or already closed");
            var data = await Get_realtime_price(findTrade.Symbol);
            findTrade.Close_price = data;
            findTrade.Close_datetime = DateTime.Now;
            findTrade.Open = false;
            await _dbcontext.SaveChangesAsync();
            var TradeDto = _mapper.Map<TradeDto>(findTrade);
            return TradeDto;
        }
        private string GetUserId()
        {
            return _httpContextAccessor!.HttpContext!.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        }
        private async Task<List<decimal>> GetPNL(bool isOpen)
        {
            var userId = GetUserId();
            return await _dbcontext.Trades.Where(x => x.UserId == userId && x.Open == isOpen)
                                                 .Select(trade => (isOpen ? trade.Open_price : trade.Close_price!.Value) * trade.Quantity)
                                                 .ToListAsync();
        }

        private async Task<decimal> Get_realtime_price(string symbol)
        {
            var url = $"https://data.messari.io/api/v1/assets/{symbol}/metrics/market-data";
            var response = await _httpClient.GetStringAsync(url) ?? throw new NotFoundException("not found");
            dynamic marketData = JsonConvert.DeserializeObject(response)!;
            decimal price = marketData?.data.market_data.price_usd;
            return price;

        }
    }
}