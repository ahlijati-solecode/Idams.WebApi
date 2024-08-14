using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Requests
{
    public class SendMailRequest
    {
        public string AppFk { get; set; } = null!;
        public string EmailCode { get; set; } = null!;
        public string TransNo { get; set; } = null!;
        public string ActionBy { get; set; } = null!;
        public string EmailSender { get; set; } = null!;
        public List<BaseKeyValueObject> ListJson { get; set; } =  new List<BaseKeyValueObject>();
        public string To { get; set; } = null!;
        public string? Cc { get; set; } 
        public string? Bcc { get; set; }
    }

    public class BaseKeyValueObject
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
}


