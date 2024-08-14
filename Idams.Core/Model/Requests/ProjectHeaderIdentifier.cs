using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Requests
{
    public class ProjectHeaderIdentifier
    {
        public string ProjectId { get; set; }
        public int ProjectVersion { get; set; }
    }

    public class MultipleProjectHeader
    {
        public List<ProjectHeaderIdentifier> Projects { get; set; }
    }
}
