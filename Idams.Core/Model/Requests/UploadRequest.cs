using Idams.Core.Attributes;
using Microsoft.AspNetCore.Http;

namespace Idams.Core.Model.Requests
{
    public class UploadRequest
    {
        public string ProjectActionId { get; set; }

        [MaxFileSize(70 * 1024 * 1024)]
        [AllowedExtensions(".pdf", ".jpg", ".jpeg", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".xlsm", "xlm")]
        public IFormFile File { get; set; }
    }

    public class UploadRequiredDocRequest : UploadRequest
    {
        public string DocDescriptionId { get; set; }
    }

    public class UploadTemplateFollowUpRequest
    {
        [AllowedExtensions(".xls", ".xlsx")]
        public IFormFile File { get; set; }
    }
}
