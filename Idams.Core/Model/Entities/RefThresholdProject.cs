using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class RefThresholdProject
    {
        public RefThresholdProject()
        {
            RefTemplateDocuments = new HashSet<RefTemplateDocument>();
            RefTemplates = new HashSet<RefTemplate>();
        }

        public string ThresholdNameParId { get; set; } = null!;
        public int ThresholdVersion { get; set; }
        public decimal? Capex1 { get; set; }
        public string? MathOperatorParId { get; set; }
        public decimal? Capex2 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public bool? Deleted { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<RefTemplateDocument> RefTemplateDocuments { get; set; }
        public virtual ICollection<RefTemplate> RefTemplates { get; set; }
    }
}
