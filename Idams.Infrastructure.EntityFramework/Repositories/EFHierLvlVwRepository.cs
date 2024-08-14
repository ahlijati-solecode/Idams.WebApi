using Idams.Core.Enums;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFHierLvlVwRepository : EFRepository<VwUserHierLvl, int, List<RoleEnum>>, IHierLvlVwRepository
    {
        protected readonly IdamsDbContext _dbContext;
        public EFHierLvlVwRepository(IdamsDbContext context, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _dbContext = context;// serviceProvider.GetService<IdamsDbContext>() ?? throw new ArgumentNullException(nameof(IdamsDbContext));
        }
        public Task<VwUserHierLvl?> GetData(string? OrgUnitID)
        {
            try
            {
                return _dbContext.VwUserHierLvls.AsNoTracking().Where(x => x.OrgUnitId == OrgUnitID).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Task<VwDistinctHierLvlType?> GetHierLvlLabel()
        {
            try
            {
                return _dbContext.VwDistinctHierLvlTypes.AsNoTracking().FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<HierLvlDto>> GetDistinctHierLvl2List()
        {
            return await _dbContext.VwShuhier03s.AsNoTracking().Select(x => new HierLvlDto() {Key = x.HierLvl2, Value = x.HierLvl2Desc}).Distinct().ToListAsync();
        }

        public async Task<List<HierLvlDto>> GetDistinctHierLvl3List()
        {
            return await _dbContext.VwShuhier03s.AsNoTracking().Select(x => new HierLvlDto() { Key = x.HierLvl3, Value = x.HierLvl3Desc}).Distinct().ToListAsync();
        }

        public async Task<List<HierLvlDto>> GetDistinctHierLvl2List(List<string> hierlvl3s)
        {
            return await _dbContext.VwShuhier03s.AsNoTracking().Where(n => hierlvl3s.Contains(n.HierLvl3))
                .Select(x => new HierLvlDto()
                {
                    Key = x.HierLvl2,
                    Value = x.HierLvl2Desc
                }).Distinct().ToListAsync();
        }

        public async Task<List<HierLvlDto>> GetDistinctHierLvl3List(string hierlvl2)
        {
            return await _dbContext.VwShuhier03s.AsNoTracking()
                .Where(x => x.HierLvl2 == hierlvl2)
                .Select(x => new HierLvlDto() { Key = x.HierLvl3, Value = x.HierLvl3Desc }).Distinct().ToListAsync();
        }

        public async Task<List<HierLvlDto>> GetHierLvl4List(string hierlvl3)
        {
            var ret = await _dbContext.VwShuhier03s.AsNoTracking()
                .Where(x => x.HierLvl3 == hierlvl3)
                .ToListAsync();

            return ret.Select(x =>
            {
                HierLvlDto ret = new HierLvlDto
                {
                    Key = x.HierLvl4,
                    Value = HierLvlDto.FormatHierLvl4(x)
                };
                return ret;
            }).ToList();
        }

        public async Task<List<VwShuhier03>> GetShuHier03(List<string> entityIds)
        {
            return await _dbContext.VwShuhier03s.AsNoTracking().Where(n => entityIds.Contains(n.EntityId)).ToListAsync();
        }
    }
}
