using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Requests
{
    public class ProjectListPagedRequest : BasePagedRequest
    {
        public string? ProjectName { get; set; }
        public string? HierLvl2Desc { get; set; }
        public string? HierLvl3Desc { get; set; }
        public string? HierLvl4 { get; set; }
        public string? Threshold { get; set; }
        public string? DrillingCost { get; set; }
        public string? FacilitiesCost { get; set; }
        public string? Capex { get; set; }
        public string? EstFIDApproved { get; set; }
        public string? RKAP { get; set; }
        public string? Stage { get; set; }
        public string? WorkflowName { get; set; }
        public string? ProjectOnStream { get; set; }
        public string? NetPresentValue { get; set; }
        public string? InternalRateOfReturn { get; set; }
        public string? ProfitabilityIndex { get; set; }
        public string? Oil { get; set; }
        public string? Gas { get; set; }
        public string? OilEquivalent { get; set; }
        public string? InitiationDate { get; set; }
        public string? EndDate { get; set; }
        public string? WorkflowActionName { get; set; }
        public string? Status { get; set; }
        public string? UpdatedDate { get; set; }
        public string? FidcodeLike { get; set; }

        #region dropdown filter
        public string? Revision { get; set; }
        public string? HierLvl2DropDown { get; set; }
        public string? ThresholdDropDown { get; set; }
        public string? StageDropdown { get; set; }
        public string? WorkflowType { get; set; }
        public string? SubCriteria { get; set; }
        #endregion
    }
}
