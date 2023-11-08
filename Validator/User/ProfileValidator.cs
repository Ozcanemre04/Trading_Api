using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using trading_app.dto.Profile;

namespace trading_app.Validator.User
{
    public class ProfileValidator:AbstractValidator<ProfileUpdateDto>
    {
        public ProfileValidator(){

             RuleFor(u => u.FirstName)
            .Length(3,40)
            .Matches(@"^[A-Z][a-zA-Z\s]*$").WithMessage("Invalid firstname: first letter must be uppercase,number is not allowed");

            RuleFor(u => u.LastName)
            .Length(3,40)
            .Matches(@"^[A-Z][a-zA-Z\s]*$").WithMessage("Invalid Lastname: first letter must be uppercase,number is not allowed");

            RuleFor(u => u.Adress)
            .Length(3,100);

            RuleFor(u => u.Username)
            .Length(3,40)
            .Matches(@"^[A-Z][A-Za-z0-9_]*$").WithMessage("Invalid Username: first letter must be uppercase");
        }
    }
}