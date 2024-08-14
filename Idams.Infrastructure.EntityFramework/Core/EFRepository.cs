using Idams.Core.Enums;
using Idams.Core.Extenstions;
using Idams.Core.Model;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;
using Idams.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Idams.Infrastructure.EntityFramework.Core
{
    public class EFRepository<TEntity, TKey, TRole> : IBaseRepository<TEntity, TKey>
        where TEntity : class
        where TRole : List<RoleEnum>
    {
        protected readonly IdamsDbContext _context;
        protected readonly ICurrentUserService _currentUserService;
        private string[] _inlcudes;
        protected readonly IConfiguration _configuration;
        public bool IsAuditEntity { get; set; } = true;


        public string BuildQuery(string sql, Dictionary<string, object> values)
        {
            var query = sql;
            foreach (var item in values)
            {
                query = query.Replace($"@{item.Key}", item.Value?.ToString());
            }
            return query.ToString();
        }


        public void Includes(params string[] inlcudes)
        {
            _inlcudes = inlcudes;
        }
        protected bool NoTraciking { get; set; }

        public virtual IQueryable<TEntity> GetAll()
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (_inlcudes != null)
                foreach (var item in _inlcudes)
                {
                    query = query.Include(item);
                }
            if (NoTraciking)
                query = query.AsNoTracking();
            return query;
        }
        protected string GetCurrentUser
        {
            get { return _currentUserService.CurrentUser?.Id ?? String.Empty; }
        }
        public EFRepository(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            else
            {
                _context = serviceProvider.GetService<IdamsDbContext>();
                _currentUserService = serviceProvider.GetService<ICurrentUserService>();
                _configuration = serviceProvider.GetService<IConfiguration>(); 
            }
        }

        public IDbConnection OpenConnection()
        {
            SqlConnection conn;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }

        public virtual Task<TEntity> AddAsync(TEntity input)
        {
            if (input is BaseAuditEntity)
            {
                var baseAuditEntity = input as BaseAuditEntity;
                if (baseAuditEntity != null)
                {
                    baseAuditEntity.CreatedDate = DateTime.UtcNow;
                    baseAuditEntity.CreatedBy = GetCurrentUser;
                }
            }
            _context.Set<TEntity>().Add(input);
            return Task.FromResult(input);
        }

        public virtual Task<Paged<TEntity>> GetPaged(PageFilter pageFilter, TRole roles, Expression<Func<TEntity, bool>> predicate = null)
        {
            IQueryable<TEntity> query = GetAll();
            if (IsAuditEntity)
                query = query.Where(n => ((IsDeletedEntity)n).DeletedBy == null);
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            query = query.OrderBy(pageFilter.Sort);
            return Task.FromResult(new Paged<TEntity>()
            {
                Items = query.Skip((pageFilter.Page - 1) * pageFilter.Size).Take(pageFilter.Size),
                TotalItems = query.Count()
            });
        }

        public virtual Task<TEntity> DeleteAsync(TEntity input, bool permanent = false)
        {
            _context.Entry(input).State = EntityState.Modified;
            _context.Set<TEntity>().Remove(input);
            return Task.FromResult(input);
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity input)
        {
            if (input is BaseAuditEntity)
            {
                var baseAuditEntity = input as BaseAuditEntity;
                var auditEntity = baseAuditEntity as AuditEntity;

                if (auditEntity != null)
                {
                    var exitingEntity = await GetById(((IEntityKey<TKey>)auditEntity).Id);
                    if (exitingEntity != null)
                    {
                        var model = exitingEntity as AuditEntity;
                        model = MigrateData(model as TEntity, auditEntity as TEntity) as AuditEntity;
                        if (model != null)
                        {
                            model.UpdatedDate = DateTime.UtcNow;
                            model.UpdatedBy = GetCurrentUser;
                        }
                        _context.Entry(model).State = EntityState.Modified;
                        _context.Set<TEntity>().Update(model as TEntity);
                    }
                }
                else if (baseAuditEntity != null)
                {
                    _context.Entry(baseAuditEntity).State = EntityState.Modified;
                    _context.Set<TEntity>().Update(baseAuditEntity as TEntity);
                }
            }
            else
            {
                _context.Set<TEntity>().Update(input);
            }
            return input;
        }

        public virtual TEntity MigrateData(TEntity exitingEntity, TEntity newEntity)
        {
            return newEntity;
        }

        public virtual async Task<TEntity> GetById(TKey id, bool isDeleted = false)
        {
            TEntity entity = default;
            var model = Activator.CreateInstance<TEntity>();
            if (model is AuditEntity)
            {
                IQueryable<TEntity> query = _context.Set<TEntity>();
                if (_inlcudes != null)
                    foreach (var item in _inlcudes)
                    {
                        query = query.Include(item);
                    }
                query = BeforeGetById(query);
                if (NoTraciking)
                    query = query.AsNoTracking();
                if (isDeleted)
                    entity = await query.Where(n => (n as IEntityKey<TKey>).Id.Equals(id)).AsNoTracking().FirstOrDefaultAsync();
                else
                    entity = await query.Where(n => (n as IEntityKey<TKey>).Id.Equals(id)).AsNoTracking().FirstOrDefaultAsync();
            }
            else
            {
                entity = _context.Set<TEntity>().Find(id);
            }
            return entity;
        }

        protected virtual IQueryable<TEntity> BeforeGetById(IQueryable<TEntity> query)
        {
            return query;
        }
    }
}