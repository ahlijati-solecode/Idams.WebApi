using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class MdDocumentDescription
    {
        public MdDocumentDescription()
        {
            MpDocumentChecklists = new HashSet<MpDocumentChecklist>();
            TxDocuments = new HashSet<TxDocument>();
        }

        public string DocDescriptionId { get; set; } = null!;
        public string? DocCategoryParId { get; set; }
        public string? DocDescription { get; set; }
        public string? DocTypeParId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        public virtual ICollection<MpDocumentChecklist> MpDocumentChecklists { get; set; }
        public virtual ICollection<TxDocument> TxDocuments { get; set; }
    }
}
