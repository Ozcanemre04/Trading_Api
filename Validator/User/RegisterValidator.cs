using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using trading_app.dto.User;

namespace trading_app.Validator.User
{
    public class RegisterValidator:AbstractValidator<RegisterDto>
    {
        public RegisterValidator(){

            RuleFor(u => u.FirstName)
            .Length(3,40)
            .NotEmpty()
            .NotNull()
            .WithMessage("firstname is required")
            .Matches(@"^[A-Z][a-zA-Z\s]*$").WithMessage("Invalid firstname: first letter must be uppercase,number is not allowed");

            RuleFor(u => u.LastName)
            .Length(3,40)
            .NotEmpty()
            .NotNull()
            .WithMessage("lastname is required")
            .Matches(@"^[A-Z][a-zA-Z\s]*$").WithMessage("Invalid Lastname: first letter must be uppercase,number is not allowed");

            RuleFor(u => u.Adress)
            .Length(3,100)
            .NotEmpty()
            .NotNull()
            .WithMessage("adress is required");
            
            RuleFor(u => u.Password)
            .NotEmpty()
            .NotNull().WithMessage("Password is required");

            RuleFor(u => u.Email)
            .NotEmpty()
            .NotNull()
            .WithMessage("Email is required")
            .Length(3,60)
            .EmailAddress().WithMessage("invalid Email");

            RuleFor(u => u.Username)
            .Length(3,40)
            .NotEmpty()
            .NotNull()
            .WithMessage("Username is required")
            .Matches(@"^[A-Z][A-Za-z0-9_]*$").WithMessage("Invalid Username: first letter must be uppercase");
        }
    }
}