using Dapper;
using Idams.Core.Extenstions;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Infrastructure.EntityFramework.Core
{
    public abstract class BaseRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;
        protected readonly IdamsDbContext _dbContext;

        public BaseRepository(IConfiguration configuration, ICurrentUserService currentService, IdamsDbContext dbContext)
        {
            _configuration = configuration;
            _currentUserService = currentService;
            _dbContext = dbContext;
        }

        protected string GetCurrentUser => _currentUserService.CurrentUser?.Id ?? String.Empty;

        protected UserDto GetCurrentUserInfo => _currentUserService.CurrentUserInfo;

        protected async Task<Paged<TPaged>> GetPaged<TPaged>(string selectQuery, string countQuery, List<List<FilterModel>>  param, int page = 1, int size = 10, string sort = "id asc")
        {
            using var connection = OpenConnection();
            var filterSql = FilterModel.BuildFilter(param);

            var metaData = InsertMetaData(selectQuery, filterSql, page, size, sort);

            var querySQL = BuildQuery(BaseQueries.BasePagedQuery, metaData);
            var items = await connection.QueryAsync<TPaged>(querySQL);
            metaData["select"] = countQuery;
            querySQL = BuildQuery(BaseQueries.BaseCountQuery, metaData);
            var count = await connection.QueryFirstAsync<int>(querySQL);

            return new Paged<TPaged>()
            {
                TotalItems = count,
                Items = items
            };
        }

        protected async Task<IEnumerable<TResult>> ExecuteQueryAsync<TResult>(string query)
        {
            using var connection = OpenConnection();
            return await connection.QueryAsync<TResult>(query);
        }

        public string BuildQuery(string sql, Dictionary<string, object> values)
        {
            var query = sql;
            foreach (var item in values)
            {
                query = query.Replace($"@{item.Key}", item.Value?.ToString());
            }
            return query.ToString();
        }

        protected virtual Dictionary<string, object> InsertMetaData(string selectQuery, string filterSql, int page, int size, string sort)
        {
            var metaData = new Dictionary<string, object>
            {
                { "select", selectQuery },
                { "sort", sort },
                { "offset", ((page - 1) * size) },
                { "limit", size },
                { "filter", filterSql }
            };

            return metaData;
        }

        protected IDbConnection OpenConnection()
        {
            SqlConnection conn;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}
