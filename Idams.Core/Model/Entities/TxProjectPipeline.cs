using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxProjectPipeline
    {
        public string ProjectPipelineId { get; set; } = null!;
        public string? ProjectId { get; set; }
        public int? ProjectVersion { get; set; }
        public string? FieldServiceParId { get; set; }
        public int? PipelineCount { get; set; }
        public decimal? PipelineLenght { get; set; }
        public string? PipelineLenghtUoM { get; set; }

        public virtual TxProjectHeader? Project { get; set; }
    }
}
