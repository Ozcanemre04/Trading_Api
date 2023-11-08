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
            try
            {
                var response = await _tradeService.AllTrades();
                return Ok(response);

            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("{id:Guid}")]
        [Authorize]
        public async Task<ActionResult<TradeDto>> OneTrade([FromRoute] Guid id)
        {
            try
            {

                var response = await _tradeService.GetOneTrade(id);
                if (response == null)
                {
                    return NotFound();
                }
                return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("open_trade")]
        [Authorize]
        public async Task<ActionResult<TradeDto>> AllOpenTrade()
        {
            try
            {
                var response = await _tradeService.GetAllOpenTrades();
                return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("closed_trade")]
        [Authorize]
        public async Task<ActionResult<TradeDto>> AllClosedTrade()
        {
            try
            {
                var response = await _tradeService.GetAllClosedTrades();
                return Ok(response);
            }
            catch
            {
                return BadRequest();
            }

        }

        [HttpGet("open_pnl")]
        [Authorize]
        public async Task<ActionResult<TradeDto>> OpenPnl()
        {
            try
            {
                var response = await _tradeService.GetOpenpnl();
                return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("closed_pnl")]
        [Authorize]
        public async Task<ActionResult<TradeDto>> ClosePnl()
        {
            try
            {
                var response = await _tradeService.GetClosedpnl();
                return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("Open_trade")]
        [Authorize]
        public async Task<ActionResult<TradeDto>> OpenTrade(AddTradeDto addTradeDto)
        {
            try
            {

                var response = await _tradeService.OpenTrade(addTradeDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Close_trade")]
        [Authorize]
        public async Task<ActionResult<TradeDto>> CloseTrade(Guid id)
        {
            try
            {
                var response = await _tradeService.CloseTrade(id);
                return Ok(response);

            }
            catch (Exception ex)
            {
                if (ex.Message == "not found or already closed")
                {

                    return NotFound(ex.Message);
                }
                return BadRequest(ex.Message);

            }
        }
    }
}