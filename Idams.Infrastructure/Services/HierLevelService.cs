using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Infrastructure.Services
{
    public class HierLevelService : IHierLevelService
    {
        private readonly IHierLvlVwRepository _iHierLvlVwRepository;

        public HierLevelService(IHierLvlVwRepository iHierLvlVwRepository)
        {
            _iHierLvlVwRepository = iHierLvlVwRepository;
        }


        public async Task<VwUserHierLvl?> GetData(string OrgUnitID)
        {
            var result = await _iHierLvlVwRepository.GetData(OrgUnitID);
            return result;
        }

        public async Task<VwHierLvlType?> GetHierLvlLabel()
        {
            var result = await _iHierLvlVwRepository.GetHierLvlLabel();
            var ret = new VwHierLvlType()
            {
                HierLvl2 = result?.HierLvl2Type,
                HierLvl3 = result?.HierLvl3Type,
                HierLvl4 = result?.HierLvl4Type,
            };
            return ret;
        }
    }
}
