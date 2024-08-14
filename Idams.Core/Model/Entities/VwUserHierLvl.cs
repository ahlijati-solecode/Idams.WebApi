using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class VwUserHierLvl
    {
        public string OrgUnitId { get; set; } = null!;
        public string? Lvl1EntityId { get; set; }
        public string? Lvl1EntityName { get; set; }
        public string? Lvl2EntityId { get; set; }
        public string? Lvl2EntityName { get; set; }
        public string? Lvl3EntityId { get; set; }
        public string? Lvl3EntityName { get; set; }
    }
}
