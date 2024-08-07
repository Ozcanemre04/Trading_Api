using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using trading_app.dto.email;

namespace trading_app.Validator.User
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordValidator(){
            RuleFor(u=>u.Email)
            .NotNull()
            .NotEmpty()
            .WithMessage("email must be not empty or null")
            .EmailAddress()
            .WithMessage("must be a valid email adress");

            RuleFor(u=>u.Token)
            .NotNull()
            .NotEmpty()
            .WithMessage("token must be not empty or null");

            RuleFor(u=>u.NewPassword)
            .NotNull()
            .NotEmpty()
            .WithMessage("password must be not empty or null")
            .MinimumLength(5)
            .WithMessage("the minimun length of password must be 5")
            .Equal(u=>u.ConfirmPassword);

            RuleFor(u=>u.ConfirmPassword)
            .NotNull()
            .NotEmpty()
            .WithMessage("password must be not empty or null")
            .MinimumLength(5)
            .WithMessage("the minimun length of password must be 5")
            .Equal(u=>u.NewPassword);
        }
    }
}