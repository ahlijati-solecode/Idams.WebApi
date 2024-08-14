using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class MenuDto
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public bool Editable { get; set; }
        public bool Alldata { get; set; }
        public bool Create { get; set; }
        public bool Draft { get; set; }
        public List<ChildMenuDto> children { get; set; }
    }
    public class ChildMenuDto
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public bool Editable { get; set; }
        public bool Alldata { get; set; }
    }
}
