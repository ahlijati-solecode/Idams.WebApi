using Idams.Core.Model.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Services
{
    public interface IEmailService
    {
        Task SendMailAsync(List<SendMailRequest> sendMailRequests);
    }
}
