using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Repositories
{
    public interface IHierLvlVwRepository
    {
        Task<VwUserHierLvl?> GetData(string? OrgUnitID);
        Task<VwDistinctHierLvlType?> GetHierLvlLabel();
        Task<List<HierLvlDto>> GetDistinctHierLvl2List();
        Task<List<HierLvlDto>> GetDistinctHierLvl3List();
        Task<List<HierLvlDto>> GetDistinctHierLvl2List(List<string> hierlvl3);
        Task<List<HierLvlDto>> GetDistinctHierLvl3List(string hierlvl2);
        Task<List<HierLvlDto>> GetHierLvl4List(string hierlvl3);
        Task<List<VwShuhier03>> GetShuHier03(List<string> entityIds);
    }
}
