using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Responses
{
    public class MdDocumentPagedResponse
    {
        public string DocDescriptionId { get; set; } = null!;
        public string? DocCategory { get; set; }
        public string? DocDescription { get; set; }
        public string? DocType { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
