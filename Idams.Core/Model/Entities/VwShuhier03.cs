using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class VwShuhier03
    {
        public string EntityId { get; set; } = null!;
        public string? CompanyCodeDesc { get; set; }
        public string? HierLvl2 { get; set; }
        public string? HierLvl2Desc { get; set; }
        public string? HierLvl3 { get; set; }
        public string? HierLvl3Desc { get; set; }
        public string? HierLvl4 { get; set; }
        public string? HierLvl4Desc { get; set; }
    }
}
