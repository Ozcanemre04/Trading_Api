using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trading_app.dto.trade
{
    public class TradeDto
    {
        public Guid Id {get;set;}
        public required string Symbol {get;set;}
        public required decimal Quantity {get;set;}
        public required decimal Open_price {get;set;}
        public decimal? Close_price {get;set;}
        public required DateTime Open_datetime {get;set;}
        public DateTime? Close_datetime {get;set;}
        public required bool Open {get;set;}
    }
}