
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using trading_app.dto.Wire;
using trading_app.interfaces;


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
            var response = await _wireService.CreateWire(addWireDto);
            return Ok(response);   
        }

        [HttpGet("current_balance")]
        [Authorize]
        public async Task<ActionResult<decimal>> CurrentBalance()
        {  
            var response = await _wireService.CurrentBalance();
            return Ok(response);   
        }
    }
}