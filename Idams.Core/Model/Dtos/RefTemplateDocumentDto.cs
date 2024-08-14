using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class RefTemplateDocumentDto
    {
        public string TemplateId { get; set; } = null!;
        public int TemplateVersion { get; set; }
        public string ThresholdNameParId { get; set; } = null!;
        public int ThresholdVersion { get; set; }
        public string WorkflowSequenceId { get; set; } = null!;
        public string WorkflowActionId { get; set; } = null!;
        public string? DocGroupParId { get; set; }
        public int? DocGroupVersion { get; set; }
        public List<MpDocumentChecklistPagedDto> DocumentChecklist { get; set; }
    }
}
