using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class MdParamater
    {
        public MdParamater()
        {
            MdParamaterLists = new HashSet<MdParamaterList>();
        }

        public string Schema { get; set; } = null!;
        public string ParamId { get; set; } = null!;
        public string? ParamDesc { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual ICollection<MdParamaterList> MdParamaterLists { get; set; }
    }
}
