using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trading_app.dto.User
{
    public class LoginResponseDto
    {
        public string? Message { get; set; }
        public string? AccessToken { get; set; }
        public string? refreshToken { get; set; }
    }
}