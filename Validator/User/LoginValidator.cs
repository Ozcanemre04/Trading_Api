using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using trading_app.dto.User;

namespace trading_app.Validator.User
{
    public class LoginValidator:AbstractValidator<LoginDto>
    {
        public LoginValidator(){
            RuleFor(u => u.Email)
            .NotEmpty()
            .NotNull()
            .WithMessage("Email is required")
            .Length(3,60)
            .EmailAddress().WithMessage("invalid Email");

            RuleFor(u => u.Password)
            .NotEmpty()
            .NotNull().WithMessage("Password is required");
        }
    }
}