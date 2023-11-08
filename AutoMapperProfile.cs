
using AutoMapper;
using trading_app.dto.Profile;
using trading_app.dto.trade;
using trading_app.dto.User;
using trading_app.dto.Wire;
using trading_app.models;

namespace trading_app
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile(){
            //auth
            CreateMap<RegisterDto,ApplicationUser>();
            CreateMap<ApplicationUser,RegisterResponseDto>();
            //wire
            CreateMap<AddWireDto,Wire>();
            CreateMap<Wire,WireDto>();
            //trade
            CreateMap<Trade,TradeDto>();
            CreateMap<AddTradeDto,Trade>();
            //profile
            CreateMap<ApplicationUser,ProfileDto>();
            CreateMap<ProfileUpdateDto,ApplicationUser>().ForAllMembers(opts => opts.Condition((src,dest,srcMember)=> srcMember != null));
            
        }
    }
}