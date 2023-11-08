

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace trading_app.models
{
    public class RefreshToken
    {
        [Key]
        public Guid Id {get;set;}
        public required string Refreshtoken {get;set;}
        public DateTime Expires {get;set;}

        [ForeignKey("ApplicationUser")]
        public required string UserId {get;set;}
        public required ApplicationUser applicationUser;
    }
}