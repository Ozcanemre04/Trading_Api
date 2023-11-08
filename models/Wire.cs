using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Serilog.Debugging;


namespace trading_app.models
{
    public class Wire
    {
        [Key]
        public Guid Id {get;set;}
        private decimal _amount;
        public decimal Amount{ get{return Withdrawal ? -_amount : _amount;}set{_amount = value;}}
        public bool Withdrawal {get;set;}

        [ForeignKey("ApplicationUser")]
        public required string UserId {get;set;}
        public required ApplicationUser applicationUser;
    }
}