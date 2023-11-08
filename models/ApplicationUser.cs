

using Microsoft.AspNetCore.Identity;

namespace trading_app.models
{
    public class ApplicationUser:IdentityUser
    {
        public required string FirstName {get;set;}
        public required string LastName {get;set;}
        public required string Adress {get;set;}

        public RefreshToken? Refreshtoken {get; set;}

        public ICollection<Wire>? Wires {get;set;}
        public ICollection<Trade>? Trades {get;set;}
    }
}