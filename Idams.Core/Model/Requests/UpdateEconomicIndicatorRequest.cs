using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Requests
{
    public class UpdateEconomicIndicatorRequest
    {
        public string ProjectId { get; set; }
        public int ProjectVersion { get; set; }
        public decimal? NetPresentValue { get; set; }
        public decimal? InternalRateOfReturn { get; set; }
        public decimal? ProfitabilityIndex { get; set; }
        public decimal? DiscountedPayOutTime { get; set; }
        public decimal? Pvin { get; set; }
        public decimal? Pvout { get; set; }
        public decimal? BenefitCostRatio { get; set; }
        public string Section { get; set; }
        public bool? SaveLog { get; set; }
    }
}
