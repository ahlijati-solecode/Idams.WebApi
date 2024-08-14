using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class FollowUpDropDownDto
    {
        public FollowUpDropDownDto()
        {
            Aspect = new();
            RiskLevel = new();
        }

        public List<KeyValuePair<string, string>> Aspect { get; set; }
        public List<KeyValuePair<string, string>> RiskLevel { get; set; }
    }
}
