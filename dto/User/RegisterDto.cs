using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trading_app.dto.User
{
    public class RegisterDto
    {
        public  string? FirstName {get;set;}
        public  string? LastName {get;set;}
        public  string? Adress {get;set;}
        public  string? Username {get;set;}
        public  string? Email {get;set;}
        public  string? Password {get;set;}
    }
}