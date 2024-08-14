using AutoMapper;
using Idams.Core.Extenstions;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Requests;
using Idams.Core.Model.Responses;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFFollowUpRepository : BaseRepository, IFollowUpRepository
    {
        private readonly IMapper _mapper;

        public EFFollowUpRepository(IConfiguration configuration,
            ICurrentUserService currentUserService,
            IdamsDbContext dbContext,
            IMapper mapper)
            : base(configuration, currentUserService, dbContext)
        {
            _mapper = mapper;
        }

        public async Task<FollowUpDropDownDto> GetDropdownList()
        {
            FollowUpDropDownDto ret = new();
            var aspects = await _dbContext.MdParamaterLists.AsNoTracking().Where(n => n.Schema == "idams" && n.ParamId == "FollowUpAspect")
                .OrderBy(n => n.ParamValue1).ToListAsync();
            foreach (var aspect in aspects)
            {
                ret.Aspect.Add(new KeyValuePair<string, string>(aspect.ParamListId, aspect.ParamValue1Text));
            }
            var riskLevels = await _dbContext.MdParamaterLists.AsNoTracking().Where(n => n.Schema == "idams" && n.ParamId == "RiskLevel")
                .OrderBy(n => n.ParamValue1).ToListAsync();
            foreach (var riskLevel in riskLevels)
            {
                ret.RiskLevel.Add(new KeyValuePair<string, string>(riskLevel.ParamListId, riskLevel.ParamValue1Text));
            }
            return ret;
        }

        public async Task<List<FollowUpDetailResponse>> GetFollowUpList(string projectActionId)
        {
            var aspectList = from pr in _dbContext.MdParamaterLists
                             where pr.Schema == "idams" && pr.ParamId == "FollowUpAspect"
                             select pr;
            var riskLevels = from pr in _dbContext.MdParamaterLists
                             where pr.Schema == "idams" && pr.ParamId == "RiskLevel"
                             select pr;
            var ret = await (from fu in _dbContext.TxFollowUps
                             join asp in aspectList on fu.FollowUpAspectParId equals asp.ParamListId
                             join rk in riskLevels on fu.RiskLevelParId equals rk.ParamListId
                             where fu.ProjectActionId == projectActionId
                             select new FollowUpDetailResponse
                             {
                                 FollowUpId = fu.FollowUpId,
                                 ProjectActionId = fu.ProjectActionId,
                                 FollowUpAspectParId = fu.FollowUpAspectParId,
                                 Notes = fu.Notes,
                                 PositionFunction = fu.PositionFunction,
                                 Recommendation = fu.Recommendation,
                                 ReviewerName = fu.ReviewerName,
                                 ReviewResult = fu.ReviewResult,
                                 RiskDescription = fu.RiskDescription,
                                 RiskLevelParId = fu.RiskLevelParId,
                                 FollowUpAspect = asp.ParamValue1Text,
                                 RiskLevel = rk.ParamValue1Text
                             }).AsNoTracking().ToListAsync();

            return ret;
        }

        public async Task<List<TxFollowUp>> AddFollowUpList(List<FollowUpRequest> followUpList)
        {
            var validateOnly1ProjectId = followUpList.GroupBy(n => n.ProjectActionId);
            if (validateOnly1ProjectId.Count() > 1)
            {
                throw new Exception("Can only add multiple follow up with same projectActionId");
            }

            var latest = await _dbContext.TxFollowUps.AsNoTracking().Where(n => n.ProjectActionId == followUpList[0].ProjectActionId)
                .OrderByDescending(n => n.FollowUpId).FirstOrDefaultAsync();

            int id = latest == null ? 1 : latest.FollowUpId + 1;

            List<TxFollowUp> newEntities = new();
            foreach(var item in followUpList)
            {
                var entity = _mapper.Map<TxFollowUp>(item);
                entity.FollowUpId = id++;
                entity.CreatedDate = DateTime.UtcNow;
                entity.CreatedBy = GetCurrentUser;
                newEntities.Add(entity);
            }
            await _dbContext.TxFollowUps.AddRangeAsync(newEntities);
            await _dbContext.SaveChangesAsync();
            return newEntities;
        }

        public async Task<TxFollowUp> UpdateFollowUp(FollowUpRequest followUp)
        {
            var item = await _dbContext.TxFollowUps.FirstOrDefaultAsync(n => n.FollowUpId == followUp.FollowUpId && n.ProjectActionId == followUp.ProjectActionId);
            if (item == null)
            {
                throw new Exception($"Specified followup not found");
            }
            followUp.CopyProperties(item);
            item.UpdatedDate = DateTime.UtcNow;
            item.UpdatedBy = GetCurrentUser;
            _dbContext.TxFollowUps.Update(item);
            await _dbContext.SaveChangesAsync();
            return item;
        }

        public async Task<bool> DeleteFollowUp(int followUpId, string projectActionId)
        {
            var item = await _dbContext.TxFollowUps.FirstOrDefaultAsync(n => n.FollowUpId == followUpId && n.ProjectActionId == projectActionId);
            if (item == null)
            {
                throw new Exception($"Specified followup not found");
            }
            _dbContext.TxFollowUps.Remove(item);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
