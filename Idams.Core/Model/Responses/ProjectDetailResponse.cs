using Idams.Core.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Responses
{
    public class ProjectDetailResponse
    {
        #region project header field
        public string ProjectId { get; set; } = null!;
        public int ProjectVersion { get; set; }
        public string? TemplateId { get; set; }
        public int? TemplateVersion { get; set; }
        public string? ProjectName { get; set; }
        public DateTime? ProjectOnStream { get; set; }
        public decimal? ParticipatingInterest { get; set; }
        public decimal? DrillingCost { get; set; }
        public string? DrillingCostUoM { get; set; }
        public decimal? FacilitiesCost { get; set; }
        public string? FacilitiesCostUoM { get; set; }
        public decimal? Capex { get; set; }
        public string? CapexUoM { get; set; }
        public DateTime? EstFidapproved { get; set; }
        public DateTime? ProposalDate { get; set; }
        public short? Rkap { get; set; }
        public bool? Revision { get; set; }
        public int? WellDrillProducerCount { get; set; }
        public int? WellDrillInjectorCount { get; set; }
        public int? WellWorkOverProducerCount { get; set; }
        public int? WellWorkOverInjectorCount { get; set; }
        public decimal? Gas { get; set; }
        public string? GasUoM { get; set; }
        public decimal? Oil { get; set; }
        public string? OilUoM { get; set; }
        public decimal? OilEquivalent { get; set; }
        public string? OilEquivalentUoM { get; set; }
        public decimal? NetPresentValue { get; set; }
        public decimal? InternalRateOfReturn { get; set; }
        public decimal? ProfitabilityIndex { get; set; }
        public decimal? DiscountedPayOutTime { get; set; }
        public decimal? Pvin { get; set; }
        public decimal? Pvout { get; set; }
        public decimal? BenefitCostRatio { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Status { get; set; }
        public string? CurrentWorkflowSequence { get; set; }
        public string? CurrentAction { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? InitiationDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? InitiationAction { get; set; }
        public string? Fidcode { get; set; }
        #endregion project header field

        #region project state
        public string? WorkflowActionId { get; set; }
        public string? OutstandingTask { get; set; }
        public string? Section { get; set; }
        public Dictionary<string, DateTime?> LogSectionUpdatedDate { get; set; }
        #endregion

        #region projectInformation
        public HierLvlDto HierLvl2 { get; set; }
        public HierLvlDto HierLvl3 { get; set; }
        public List<HierLvlDto> HierLvl4 { get; set; }
        public string? ProjectCategory { get; set; }
        public string? ProjectCriteria { get; set; }
        public string? ProjectSubCriteria { get; set; }
        public string? TemplateName { get; set; }
        public string? Threshold { get; set; }
        public bool TemplateLocked { get; set; }
        #endregion
    }
}
