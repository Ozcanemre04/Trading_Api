using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trading_app.dto.User
{
    public class RegisterResponseDto
    {
        public bool IsSucceed { get; set; }
        public required string Message { get; set; }
        
        public string? UserName { get; set; }
        public string? Email {get; set;}
        public  string? FirstName {get;set;}
        
        public  string? LastName {get;set;}
        public  string? Adress {get;set;}
    }
}