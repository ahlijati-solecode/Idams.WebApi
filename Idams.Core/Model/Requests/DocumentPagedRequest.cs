using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Requests
{
    public class DocumentPagedRequest : BasePagedRequest
    {
        public string? DocDescription { get; set; }
        public string? DocCategory { get; set; }
        public string? DocType { get; set; }
        public string? DateModified { get; set; }
    }

    public class DocumentManagementPagedRequest : BasePagedRequest
    {
        public List<string> Stage { get; set; } = new List<string>();
        public List<string> Regional { get; set; } = new List<string>();
        public List<string> Threshold { get; set; } = new List<string>();
        public List<string> WorkflowType { get; set; } = new List<string>();
        public List<string> DocType { get; set; } = new List<string>();
        public List<string> FileName { get; set; } = new List<string>();
        public List<string> DocCategory { get; set; } = new List<string>();
        public List<string> ProjectName { get; set; } = new List<string>();
        public List<string> Zona { get; set; } = new List<string>();
        public List<string> Zonas { get; set; } = new List<string>();
        public List<int> FileSize { get; set; } = new List<int>();
        public List<string> UploadBy { get; set; } = new List<string>();
        public List<string> RKAP { get; set; } = new List<string>();
        public List<bool> Revision { get; set; } = new List<bool>();


    }
}
