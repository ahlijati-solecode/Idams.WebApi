using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Filters
{
    public class ParamFilter
    {
        public string Schema { get; set; }
        public string ParamID { get; set; }
        public string ParamListID { get; set; }

        public ParamFilter(string schema, string paramID, string paramListID)
        {
            Schema = schema;
            ParamID = paramID;
            ParamListID = paramListID;
        }
    }
}
