using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class MdDocumentPagedDto
    {
        public string DocDescriptionId { get; set; } = null!;
        public string? DocCategoryParId { get; set; }
        public string? DocCategory { get; set; }
        public string? DocDescription { get; set; }
        public string? DocTypeParId { get; set; }
        public string? DocType { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
