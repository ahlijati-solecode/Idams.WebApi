using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxProjectPlatform
    {
        public string ProjectPlateformId { get; set; } = null!;
        public string? ProjectId { get; set; }
        public int? ProjectVersion { get; set; }
        public int? PlatformCount { get; set; }
        public int? PlatformLegCount { get; set; }

        public virtual TxProjectHeader? Project { get; set; }
    }
}
