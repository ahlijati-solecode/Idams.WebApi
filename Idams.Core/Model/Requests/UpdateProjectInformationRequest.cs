using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Requests
{
    public class UpdateProjectInformationRequest
    {
        public string ProjectId { get; set; }
        public int ProjectVersion { get; set; }
        public string? TemplateId { get; set; }
        public int? TemplateVersion { get; set; }
        public string? ProjectName { get; set; }
        public DateTime? ProjectOnStream { get; set; }
        public decimal? ParticipatingInterest { get; set; }
        public decimal? DrillingCost { get; set; }
        public decimal? FacilitiesCost { get; set; }
        public decimal? Capex { get; set; }
        public DateTime? EstFidapproved { get; set; }
        public DateTime? ProposalDate { get; set; }
        public short? Rkap { get; set; }
        public bool? Revision { get; set; }
        public string Section { get; set; }
        public bool? SaveLog { get; set; }
        public List<string> EntityIds { get; set; }

    }
}
