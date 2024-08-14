using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class MdFidcode
    {
        public string SubholdingCode { get; set; } = null!;
        public string ProjectCategory { get; set; } = null!;
        public int ApprovedYear { get; set; }
        public int Regional { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int LastNumber { get; set; }
    }
}
