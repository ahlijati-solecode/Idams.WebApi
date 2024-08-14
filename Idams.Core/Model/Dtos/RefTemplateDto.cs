using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class RefTemplateDto
    {
        public string? TemplateId { get; set; }
        public int? TemplateVersion { get; set; }
        public string? TemplateName { get; set; }
        public string? ProjectCategory { get; set; }
        public string? ProjectCriteria { get; set; }
        public string? ProjectSubCriteria { get; set; }
        public string? ThresholdName { get; set; }
        public bool? IsActive { get; set; }
        public bool? Deleted { get; set; }
        public int? TotalWorkflow { get; set; }
        public string? StartWorkflow { get; set; }
        public string? EndWorkflow { get; set; }
        public string? Status { get; set; }

        public List<RefTemplateWorkflowSequenceDto> WorkflowSequence { get; set; }

        public RefTemplateDto()
        {
            WorkflowSequence = new List<RefTemplateWorkflowSequenceDto>();
        }
    }
}
