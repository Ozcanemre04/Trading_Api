using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace trading_app.dto.email
{
    public class ResetPasswordDto
    {

        public required string Token { get; set; }

        public required string Email { get; set; }

        public required string NewPassword { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}