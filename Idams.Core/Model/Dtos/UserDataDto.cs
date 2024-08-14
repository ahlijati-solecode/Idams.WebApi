using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class UserDataDto
    {
        public string AuthUserApp { get; set; }
        public List<UserRoleDto> Roles { get; set; }
    }
}
