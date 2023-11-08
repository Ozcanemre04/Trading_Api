using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trading_app.dto.Wire;

namespace trading_app.dto.Profile
{
    public class ProfileDto
    {
        public required string Id {get;set;}
        public required string FirstName {get;set;}
        public required string LastName {get;set;}
        public required string Adress {get;set;}
        public required string Email {get;set;}
        public required string Username {get;set;}
        public required decimal CurrentBalance {get;set;}
    }
}