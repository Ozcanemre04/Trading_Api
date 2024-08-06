using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace trading_app.dto.User
{
    public class RefreshTokenDto
    {   [Required]
        [MinLength(150)]
        public required string AccessToken { get; set; }
        [Required]
        [MinLength(50)]
        public required string refreshToken { get; set; }
    }
}