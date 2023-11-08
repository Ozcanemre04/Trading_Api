using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trading_app.dto.Wire
{
    public class AddWireDto
    {
        private decimal _amount;
        public decimal Amount{ get{return Withdrawal ? -_amount : _amount;}set{_amount = value;}}
        
        public bool Withdrawal {get;set;}
    }
}