using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trading_app.dto;
using trading_app.dto.trade;

namespace trading_app.interfaces
{
    public interface ITradeService
    {
        Task<PageDto<TradeDto>> AllTrades(int pageNumber, int pageSize);
        Task<TradeDto> GetOneTrade(Guid id);
        Task<IEnumerable<TradeDto>> GetAllOpenTrades();
        Task<IEnumerable<TradeDto>> GetAllClosedTrades();
        Task<PnlDto> GetClosedpnl();
        Task<PnlDto> GetOpenpnl();
        Task<TradeDto> OpenTrade(AddTradeDto addTradeDto);
        Task<TradeDto> CloseTrade(Guid id);
    }
}