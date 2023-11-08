using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trading_app.dto.Profile;

namespace trading_app.interfaces
{
    public interface IProfileService
    {
        Task<ProfileDto> Profile();
        Task<ProfileDto> UpdateProfile(ProfileUpdateDto profileUpdateDto);
    }
}