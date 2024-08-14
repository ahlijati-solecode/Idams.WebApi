using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Entities.Custom
{
    public class ProjectListPaged
    {
        public string ProjectId { get; set; }
        public string ProjectVersion { get; set; }
        public string ProjectName { get; set; }
        public string HierLvl2 { get; set; }
        public string HierLvl2Desc { get; set; }
        public string HierLvl3 { get; set; }
        public string HierLvl3Desc { get; set; }
        public string HierLvl4 { get; set; }
        public string ThresholdParId { get; set; }
        public string Threshold { get; set; }
        public string? SubCriteria { get; set; }
        public double DrillingCost { get; set; }
        public double FacilitiesCost { get; set; }
        public double Capex { get; set; }
        public DateTime? EstFIDApproved { get; set; }
        public int RKAP { get; set; }
        public bool Revision { get; set; }
        public int? WellDrillProducerCount { get; set; }
        public int? WellDrillInjectorCount { get; set; }
        public int? WellWorkOverProducerCount { get; set; }
        public int? WellWorkOverInjectorCount { get; set; }
        public string Stage { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowType { get; set; }
        public DateTime? ProjectOnStream { get; set; }
        public double NetPresentValue { get; set; }
        public double InternalRateOfReturn { get; set; }
        public double ProfitabilityIndex { get; set; }
        public double Oil { get; set; }
        public double Gas { get; set; }
        public double OilEquivalent { get; set; }
        public double PVIn { get; set; }
        public double PVOut { get; set; }
        public double BenefitCostRatio { get; set; }
        public DateTime? InitiationDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string WorkflowActionName { get; set; }
        public string Status { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Fidcode { get; set; }
    }
}
