using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxUserTableConfig
    {
        public string UserId { get; set; } = null!;
        public string? Config { get; set; }
    }
}
