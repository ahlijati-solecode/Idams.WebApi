using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Responses
{
    public class ProjectDocumentGroupResponse
    {
        public string ProjectActionId { get; set; }
        public string DocGroupName { get; set; }
        public List<DocumentDetailResponse> RequiredDocs { get; set; }
        public List<DocumentDetailResponse> SupportingDocs { get; set; }
    }

    public class DocumentDetailResponse
    {
        public string RequiredName { get; set; }
        public string DocDescriptionId { get; set; }
        public DocumentDto UploadedDocument { get; set; }
    }
}
