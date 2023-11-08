using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace trading_app.dto.Wire
{
    public class WireDto
    {
        public Guid Id {get;set;}
        private decimal _amount;
        public decimal Amount{ get{return Withdrawal ? -_amount : _amount;}set{_amount = value;}}
        public bool Withdrawal {get;set;}
        public required string UserId {get;set;}

    }
}