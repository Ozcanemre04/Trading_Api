using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace trading_app.models
{
    public class Trade
    {
        [Key]
        public Guid Id {get;set;}
        
        public required string Symbol {get;set;}
        public required decimal Quantity {get;set;}
        public required decimal Open_price {get;set;}
        public decimal? Close_price {get;set;}
        public required DateTime Open_datetime {get;set;} = DateTime.Now;
        public DateTime? Close_datetime {get;set;}
        public required bool Open {get;set;} = true;

        [ForeignKey("ApplicationUser")]
        public required string UserId {get;set;}
        public required ApplicationUser applicationUser;
    }
}