using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxProjectComment
    {
        public string ProjectId { get; set; } = null!;
        public int ProjectCommentId { get; set; }
        public string? ProjectActionId { get; set; }
        public string? Comment { get; set; }
        public string? EmpName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        public virtual TxProjectAction? ProjectAction { get; set; }
    }
}
