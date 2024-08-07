using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trading_app.dto.email;

namespace trading_app.interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailSendDto emailSendDto);
    }
}