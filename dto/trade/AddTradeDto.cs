using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trading_app.dto.trade
{
    public class AddTradeDto
    {
        public  string? Symbol {get;set;}
        public  decimal? Quantity {get;set;}
        
        
    }
}