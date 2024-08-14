using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxApproval
    {
        public int ApprovalId { get; set; }
        public string ProjectActionId { get; set; } = null!;
        public string? Notes { get; set; }
        public string? ApprovalBy { get; set; }
        public string? EmpName { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalStatusParId { get; set; }

        public virtual TxProjectAction ProjectAction { get; set; } = null!;
    }
}
