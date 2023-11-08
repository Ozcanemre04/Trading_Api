using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using trading_app.dto.Wire;
using trading_app.interfaces;
using trading_app.Validator.Wire;

namespace trading_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WireController : ControllerBase
    {
        private readonly IWireService _wireService;

        public WireController(IWireService wireService)
        {
            _wireService = wireService;
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<WireDto>> CreateWire(AddWireDto addWireDto)
        {
            try
            {
                var response = await _wireService.CreateWire(addWireDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("current_balance")]
        [Authorize]
        public async Task<ActionResult<decimal>> CurrentBalance()
        {
            try
            {
                var response = await _wireService.CurrentBalance();
                return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}