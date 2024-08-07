using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace trading_app.dto.email
{
    public class ConfirmEmailDto
    {
        [Required]
        public required string Token { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        
    }
}