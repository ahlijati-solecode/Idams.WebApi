using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class ApprovalDetailDto
    {
        public ApprovalDetailDto()
        {
            ApprovalHistory = new();
        }

        public string? Status { get; set; }
        public string? EmpName { get; set; }
        public DateTime? Date { get; set; }
        public string? Notes { get; set; }
        public List<ApprovalHistoryDetailDto> ApprovalHistory { get; set; }
    }

    public class ApprovalHistoryDetailDto
    {
        public string? EmpName { get; set; }
        public DateTime? Date { get; set; }
        public string? Notes { get; set; }
    }
}
