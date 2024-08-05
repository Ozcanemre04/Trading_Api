using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trading_app.dto.trade;
using trading_app.interfaces;
using trading_app.Validator.Trade;

namespace trading_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradeController : ControllerBase
    {
        private readonly ITradeService _tradeService;
        public TradeController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpGet("index")]
        [Authorize]
        public async Task<ActionResult<TradeDto>> AllTrade()
        {
            var response = await _tradeService.AllTrades();
            return Ok(response);
        }

        [HttpGet("{id:Guid}")]
        [Authorize]
        public async Task<ActionResult<TradeDto>> OneTrade([FromRoute] Guid id)
        {

            return Ok(await _tradeService.GetOneTrade(id));
        }

        [HttpGet("open_trade")]
        [Authorize]
        public async Task<ActionResult<TradeDto>> AllOpenTrade()
        {

            var response = await _tradeService.GetAllOpenTrades();
            return Ok(response);

        }

        [HttpGet("closed_trade")]
        [Authorize]
        public async Task<ActionResult<TradeDto>> AllClosedTrade()
        {

            var response = await _tradeService.GetAllClosedTrades();
            return Ok(response);

        }

        [HttpGet("open_pnl")]
        [Authorize]
        public async Task<ActionResult<TradeDto>> OpenPnl()
        {

            var response = await _tradeService.GetOpenpnl();
            return Ok(response);


        }

        [HttpGet("closed_pnl")]
        [Authorize]
        public async Task<ActionResult<TradeDto>> ClosePnl()
        {

            var response = await _tradeService.GetClosedpnl();
            return Ok(response);


        }

        [HttpPost("Open_trade")]
        [Authorize]
        public async Task<ActionResult<TradeDto>> OpenTrade(AddTradeDto addTradeDto)
        {
            var response = await _tradeService.OpenTrade(addTradeDto);
            return Ok(response);
        }

        [HttpPut("Close_trade")]
        [Authorize]
        public async Task<ActionResult<TradeDto>> CloseTrade(Guid id)
        {
            var response = await _tradeService.CloseTrade(id);
            return Ok(response);
        }
    }
}