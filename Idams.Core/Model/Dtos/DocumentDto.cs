using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class DocumentDto
    {
        public string TransactionDocId { get; set; } = null!;
        public string? DocDescriptionId { get; set; }
        public string? DocName { get; set; }
        public string? FileExtension { get; set; }
        public int? FileSize { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
