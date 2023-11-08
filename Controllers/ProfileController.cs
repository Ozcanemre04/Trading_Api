using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trading_app.dto.Profile;
using trading_app.interfaces;
using trading_app.Validator.User;

namespace trading_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ProfileDto>> Profile()
        {
            try
            {
                var response = await _profileService.Profile();
                return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult<ProfileDto>> UpdateProfile([FromBody] ProfileUpdateDto profileUpdateDto)
        {
            try
            {
                var response = await _profileService.UpdateProfile(profileUpdateDto);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}