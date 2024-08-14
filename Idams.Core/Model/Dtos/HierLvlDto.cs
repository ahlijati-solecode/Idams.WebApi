using Idams.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class HierLvlDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public static string FormatHierLvl4(VwShuhier03 hier)
        {
            if (string.IsNullOrWhiteSpace(hier.CompanyCodeDesc))
            {
                return hier.HierLvl4Desc!;
            }
            else
            {
                return $"{hier.HierLvl4Desc} ({hier.CompanyCodeDesc})";
            }
        }
    }
}
