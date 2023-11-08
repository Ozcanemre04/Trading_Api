using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using trading_app.dto.Wire;

namespace trading_app.Validator.Wire
{
    public class WireValidator:AbstractValidator<AddWireDto>
    {
        
        public WireValidator(){
            RuleFor(u => u.Amount)
                .NotNull().NotEmpty()
                .WithMessage("Amount can not be null or empty")
                .PrecisionScale(20,2,false).WithMessage("must have 2 digits after the decimal point");
            
          
                    
        }
        
        
    }
}