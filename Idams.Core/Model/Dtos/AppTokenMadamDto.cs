using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class AppTokenMadamDto
    {
        public bool authenticated { get; set; }
        public string token { get; set; }
    }
}
