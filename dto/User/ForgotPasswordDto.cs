using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace trading_app.dto.User
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "not valid email address")]
        public required string Email { get; set; }
    }
}