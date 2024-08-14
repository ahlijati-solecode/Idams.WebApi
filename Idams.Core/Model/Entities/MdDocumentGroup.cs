using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class MdDocumentGroup
    {
        public MdDocumentGroup()
        {
            MpDocumentChecklists = new HashSet<MpDocumentChecklist>();
            RefTemplateDocuments = new HashSet<RefTemplateDocument>();
        }

        public string DocGroupParId { get; set; } = null!;
        public int DocGroupVersion { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        public virtual ICollection<MpDocumentChecklist> MpDocumentChecklists { get; set; }
        public virtual ICollection<RefTemplateDocument> RefTemplateDocuments { get; set; }
    }
}
