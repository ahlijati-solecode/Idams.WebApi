using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Filters
{
    public class DocumentChecklistFilter
    {
        public string? DocDescription { get; set; }
        public string? DocCategory { get; set; }
        public string? DocType { get; set; }
        public string? DateModified { get; set; }

    }
}
