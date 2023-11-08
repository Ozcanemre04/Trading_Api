using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using trading_app.dto.trade;

namespace trading_app.Validator.Trade
{
    public class TradeValidator: AbstractValidator<AddTradeDto>
    {
        public TradeValidator(){
            RuleFor(u=>u.Quantity)
                .NotNull().NotEmpty()
                .WithMessage("Amount can not be null or empty")
                .PrecisionScale(20,2,false).WithMessage("must have 2 digits after the decimal point");
            
            RuleFor(u=>u.Symbol)
                   .NotNull().NotEmpty().WithMessage("Amount can not be null or empty");
                   
        }
        
    }
}