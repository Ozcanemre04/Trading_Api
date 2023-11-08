using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trading_app.dto.trade
{
    public class PnlDto
    {
        public required decimal Pnl {get;set;}
        public required bool Open {get;set;}
    }
}