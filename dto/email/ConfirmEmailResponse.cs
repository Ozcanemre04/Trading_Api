using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace trading_app.dto.email
{
    public class ConfirmEmailResponse
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Message { get; set; }
    }
}