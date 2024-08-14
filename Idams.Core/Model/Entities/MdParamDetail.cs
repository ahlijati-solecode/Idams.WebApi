using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class MdParamDetail
    {
        public string ParamId { get; set; } = null!;
        public string ParamDetailId { get; set; } = null!;
        public string? ParamDetail { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
