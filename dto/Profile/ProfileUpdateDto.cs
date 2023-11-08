using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trading_app.dto.Profile
{
    public class ProfileUpdateDto
    {
        public string? FirstName {get;set;}
        public string? LastName {get;set;}
        public string? Adress {get;set;}
        
        public string? Username {get;set;}
    }
}