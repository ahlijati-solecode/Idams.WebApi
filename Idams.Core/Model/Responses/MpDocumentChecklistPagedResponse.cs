using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Responses
{
    public class MpDocumentChecklistPagedResponse
    {
        public string DocGroupParId { get; set; } = null!;
        public int DocGroupVersion { get; set; }
        public string DocId { get; set; } = null!;
        public string DocDescription { get; set; } = null!;
        public string DocCategory { get; set; } = null!;
        public string DocType { get; set; } = null!;
        public DateTime? ModifiedDate { get; set; }
    }
}
