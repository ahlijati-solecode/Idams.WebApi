using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxProjectEquipment
    {
        public string ProjectEquipmentId { get; set; } = null!;
        public string? ProjectId { get; set; }
        public int? ProjectVersion { get; set; }
        public int? EquipmentCount { get; set; }
        public string? EquipmentName { get; set; }

        public virtual TxProjectHeader? Project { get; set; }
    }
}
