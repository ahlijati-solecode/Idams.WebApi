using Idams.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Services
{
    public interface IHierLevelService
    {
        Task<VwUserHierLvl?> GetData(string OrgUnitID);
        Task<VwHierLvlType?> GetHierLvlLabel();
    }
}
