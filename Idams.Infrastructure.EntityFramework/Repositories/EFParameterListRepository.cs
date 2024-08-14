using Dapper;
using Idams.Core.Enums;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Filters;
using Idams.Core.Repositories;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFParameterListRepository : EFRepository<MdParamaterList, int, List<RoleEnum>>, IParameterListRepository
    {
        protected readonly IdamsDbContext _dbContext;
        public EFParameterListRepository(IdamsDbContext context, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _dbContext = context;// serviceProvider.GetService<IdamsDbContext>() ?? throw new ArgumentNullException(nameof(IdamsDbContext));
        }

        public Task<MdParamaterListDto?> GetParam(string schema, string? ParamID = null, string? ParamListID = null)
        {
            try
            {
                //var query = _dbContext.MdParamaterLists.AsQueryable();
                return _dbContext.MdParamaterLists.AsNoTracking()
                    .Where(x => x.Schema == schema && x.ParamId == ParamID && x.ParamListId == ParamListID)
                    .Select(x => new MdParamaterListDto
                    {
                        Schema = x.Schema,
                        ParamId = x.ParamId,
                        ParamListId = x.ParamListId,
                        ParamValue1 = x.ParamValue1,
                        ParamValue1Text = x.ParamValue1Text,
                        ParamValue2 = x.ParamValue2,
                        ParamValue2Text = x.ParamValue2Text,
                        ParamListDesc = x.ParamListDesc
                    }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<MdParamaterList>> GetParams(string schema, string paramID)
        {
            var query = _dbContext.MdParamaterLists.AsQueryable();
            return await query.AsNoTracking()
                .Where(x => x.Schema == schema && x.ParamId == paramID).ToListAsync();
        }

        public async Task<List<MdParamaterList>> GetParams(List<ParamFilter> filter)
        {
            using (var connection = OpenConnection())
            {
                List<FilterBuilderModel> param = new();
                var filterSql = GenerateFilter(filter, FilterBuilderEnum.OR);
                var items = await connection.QueryAsync<MdParamaterList>($"select * from MD_ParamaterList {filterSql}");

                return items.ToList();
            }
        }

        public string GenerateFilter(List<ParamFilter> filter, string joinOperator)
        {
            var query = string.Empty;
            var stringBuilder = new StringBuilder("");
            if (filter.Count > 0)
            {
                stringBuilder.Append(" where ");
                foreach (ParamFilter modelItem in filter)
                {
                    stringBuilder.Append(" ([Schema]='" + modelItem.Schema + "' AND ParamID='" + modelItem.ParamID + "' AND ParamListID='" + modelItem.ParamListID + "') " + joinOperator);
                }
                query = stringBuilder.ToString();
                query = query.Substring(0, query.Length - joinOperator.Length);
            }
            return query;
        }
    }
}
