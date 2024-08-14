using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class AppMenuDto
    {
        public string AdditionalInfo { get; set; }
        public Guid Id { get; set; }
        public string Caption { get; set; }
        public string Css { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; }
        public List<AppMenuChild> Child { get; set; }
    }
    public class AppMenuChild
    {
        public Guid Id { get; set; }
        public string Caption { get; set; }
        public string Css { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; }
    }
}
