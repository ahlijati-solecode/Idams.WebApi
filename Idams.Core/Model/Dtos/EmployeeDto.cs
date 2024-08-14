using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class EmployeeDto
    {
        public string EmpAccount { get; set; }
        public string EmpEmail { get; set; }
        public string EmpID { get; set; }
        public string EmpName { get; set; }
        public string ParentPosTitle { get; set; }
        public string? OrgUnitID { get; set; }
    }
}
