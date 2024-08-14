using Idams.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class UserRoleDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public RoleEnum Enum { get; set; }
    }
}
