using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxProjectLvl3Entity
    {
        public string ProjectId { get; set; } = null!;
        public string Lvl3EntityId { get; set; } = null!;
        public string? Lvl3EntityName { get; set; }

        public virtual TxProjectHeader Project { get; set; } = null!;
    }
}
