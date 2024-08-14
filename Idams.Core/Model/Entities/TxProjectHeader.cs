using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxProjectHeader
    {
        public TxProjectHeader()
        {
            LgProjectHeaderAuditTrails = new HashSet<LgProjectHeaderAuditTrail>();
            TxProjectCompressors = new HashSet<TxProjectCompressor>();
            TxProjectEquipments = new HashSet<TxProjectEquipment>();
            TxProjectMilestones = new HashSet<TxProjectMilestone>();
            TxProjectPipelines = new HashSet<TxProjectPipeline>();
            TxProjectPlatforms = new HashSet<TxProjectPlatform>();
        }

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
        public bool? IsActive { get; set; }
        public string? LastUpdateWorkflowSequence { get; set; }
        public string? Fidcode { get; set; }

        public virtual RefTemplate? Template { get; set; }
        public virtual ICollection<LgProjectHeaderAuditTrail> LgProjectHeaderAuditTrails { get; set; }
        public virtual ICollection<TxProjectCompressor> TxProjectCompressors { get; set; }
        public virtual ICollection<TxProjectEquipment> TxProjectEquipments { get; set; }
        public virtual ICollection<TxProjectMilestone> TxProjectMilestones { get; set; }
        public virtual ICollection<TxProjectPipeline> TxProjectPipelines { get; set; }
        public virtual ICollection<TxProjectPlatform> TxProjectPlatforms { get; set; }
    }
}
