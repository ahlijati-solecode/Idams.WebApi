using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Entities.Custom
{
    public class ProjectListDropdown
    {
        public Dictionary<string, string> HierLvl2 { get; set; }
        public Dictionary<string, string> HierLvl3 { get; set; }    
        public Dictionary<string, string> Threshold { get; set; }
        public Dictionary<string, string> Stage { get; set; }
        public Dictionary<string, string> Workflow { get; set; }
    }
}
