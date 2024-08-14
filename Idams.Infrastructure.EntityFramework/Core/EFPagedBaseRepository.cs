using Idams.Core.Model.Entities;
using Dapper;
using System.Text;
using Idams.Infrastructure.EntityFramework.Queries;
using Idams.Core.Enums;

namespace Idams.Infrastructure.EntityFramework.Core
{
    public abstract class EFPagedBaseRepository<TEntity, TFilter, TPaged, TRole> : EFRepository<TEntity, string, TRole>
       where TEntity : class
       where TRole : List<RoleEnum>
    {
        public abstract string SelectPagedQuery { get; }
        public abstract string CountQuery { get; }
        public virtual string JoinQuery { get; }

        public EFPagedBaseRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public async Task<IEnumerable<TResult>> ExecuteQueryAsync<TResult>(string query, string connectionString = null)
        {
            using (var connection = OpenConnection())
            {
                return await connection.QueryAsync<TResult>(query);
            }
        }

        protected virtual Dictionary<string, object> InsertMetaData(string filterSql, int page, int size, string sort)
        {
            var metaData = new Dictionary<string, object>();

            metaData.Add("sort", sort);
            metaData.Add("offset", ((page - 1) * size));
            metaData.Add("limit", size);

            metaData.Add("filter", filterSql);
            metaData.Add("join", JoinQuery);

            metaData.Add("select", $"{SelectPagedQuery}");

            return metaData;
        }



        public async Task<Paged<TPaged>> GetPaged(TFilter filter, TRole role,  int page = 1, int size = 10, string sort = "id asc")
        {
            using (var connection = OpenConnection())
            {
                List<FilterBuilderModel> param = new();
                List<FilterBuilderModel> paramRole = new();
                if (filter != null)
                {
                    BuildQuery(param, paramRole, filter, role);
                }
                var filterSql = FilterBuilderModel.filterBuilder("", param, paramRole, FilterBuilderEnum.AND);

                var metaData = InsertMetaData(filterSql, page, size, sort);

                var querySQL = BuildQuery(BaseQueries.PagedQuery, metaData);
                var dictionary = new Dictionary<string, object>
                {
                    { "@sort", sort },
                    {"@offset",((page - 1) * size) },
                    {"@limit",size },
                    {"@filter",filterSql },
                    {"@join",JoinQuery }
                };
                var parameters = new DynamicParameters(dictionary);
                var items = await connection.QueryAsync<TPaged>(querySQL, parameters);
                BeforeRenderItems(items);
                metaData["select"] = CountQuery;
                querySQL = BuildQuery(BaseQueries.Query, metaData);
                var count = await connection.QueryFirstAsync<int>(querySQL);

                return new Paged<TPaged>()
                {
                    TotalItems = count,
                    Items = items
                };
            }
        }

        public virtual void BeforeRenderItems(IEnumerable<TPaged> items)
        {

        }
        protected abstract void BuildQuery(List<FilterBuilderModel> param, List<FilterBuilderModel> paramRole, TFilter filter, TRole role);
    }

    public class FilterBuilderModel
    {
        public string? Column { get; set; }
        public string? Operator { get; set; }
        public string? Value { get; set; }

        public FilterBuilderModel(string _column, string _operator, string _value)
        {
            Column = _column;
            Operator = _operator;
            if (_value.StartsWith("'") && _value.EndsWith("'"))
            {
                _value = "'" + _value.Substring(1, _value.Length - 2).Replace("'", "''") + "'";
            }

            Value = _value;
        }

        public static string filterBuilder(string query, List<FilterBuilderModel> model, List<FilterBuilderModel> modelRole, string joinOperator)
        {
            var stringBuilder = new StringBuilder(query);
            if(model.Count > 0 || modelRole.Count > 0) stringBuilder.Append(" where ");
            if (model.Count > 0)
            {
                foreach (FilterBuilderModel modelItem in model)
                {
                    if (modelItem.Operator == "LIKE")
                    {
                        stringBuilder.Append(" LOWER(" + modelItem.Column + ") " + modelItem.Operator + " LOWER(" + modelItem.Value + ") " + joinOperator);
                    }
                    else
                    {
                        stringBuilder.Append(" " + modelItem.Column + " " + modelItem.Operator + " " + modelItem.Value + " " + joinOperator);
                    }
                }
            }
            if (modelRole.Count > 0)
            {
                stringBuilder.Append("(");
                foreach (FilterBuilderModel modelItem in modelRole)
                {
                    stringBuilder.Append(" " + modelItem.Column + " " + modelItem.Operator + " " + modelItem.Value + " " + "OR");
                }
            }

            query = stringBuilder.ToString();
            if(model.Count > 0 || modelRole.Count > 0)query = query.Substring(0, query.Length - joinOperator.Length);
            if (modelRole.Count > 0) query += ")";
            return query;
        }
    }
}
