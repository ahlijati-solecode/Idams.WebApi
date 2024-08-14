using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class MdParamaterListDto
    {
        public string Schema { get; set; } = null!;
        public string ParamId { get; set; } = null!;
        public string ParamListId { get; set; } = null!;
        public decimal? ParamValue1 { get; set; }
        public string? ParamValue1Text { get; set; }
        public decimal? ParamValue2 { get; set; }
        public string? ParamValue2Text { get; set; }
        public string? ParamListDesc { get; set; }
        public int? RowOrder { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
