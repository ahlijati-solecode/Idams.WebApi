using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class MpDocumentChecklist
    {
        public string DocGroupParId { get; set; } = null!;
        public int DocGroupVersion { get; set; }
        public string DocDescriptionId { get; set; } = null!;
        public DateTime? ModifiedDate { get; set; }

        public virtual MdDocumentDescription DocDescription { get; set; } = null!;
        public virtual MdDocumentGroup DocGroup { get; set; } = null!;
    }
}
