using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxProjectCompressor
    {
        public string ProjectCompressorId { get; set; } = null!;
        public string? ProjectId { get; set; }
        public int? ProjectVersion { get; set; }
        public string? CompressorTypeParId { get; set; }
        public int? CompressorCount { get; set; }
        public decimal? CompressorCapacity { get; set; }
        public string? CompressorCapacityUoM { get; set; }
        public decimal? CompressorDischargePressure { get; set; }
        public string? CompressorDischargePressureUoM { get; set; }

        public virtual TxProjectHeader? Project { get; set; }
    }
}
