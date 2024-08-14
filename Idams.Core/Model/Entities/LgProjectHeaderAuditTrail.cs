using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class LgProjectHeaderAuditTrail
    {
        public string ProjectId { get; set; } = null!;
        public int ProjectVersion { get; set; }
        public string Action { get; set; } = null!;
        public string? Section { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual TxProjectHeader Project { get; set; } = null!;
    }
}
