using ECC.Dados.Infra;
using ECC.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ECC.Dados.Repositorio
{
    public class EntidadeBaseRep<T> : IEntidadeBaseRep<T> where T : class, IEntidadeBase, new()
    {
        private EconomizajaContext dataContext;

        #region Properties
        protected IDbFactory DbFactory
        {
            get;
            private set;
        }

        protected EconomizajaContext DbContext
        {
            get { return dataContext ?? (dataContext = DbFactory.Init()); }
        }
        public EntidadeBaseRep(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }
        #endregion

        public virtual IQueryable<T> GetAll()
        {
            return DbContext.Set<T>();
        }

        public virtual IQueryable<T> All => GetAll();

        public virtual IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = DbContext.Set<T>();
            return includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public virtual T GetSingle(int id)
        {
            return this.FirstOrDefault(x => x.Id == id);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return All.FirstOrDefault(predicate);
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return All.Where(predicate);
        }

        public virtual void Add(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry<T>(entity);
            DbContext.Set<T>().Add(entity);
        }

        public virtual void Edit(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        public virtual void EditComCommit(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
            DbContext.Commit();
        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public virtual void DeleteAll(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                this.Delete(entity);
            }
        }

        public virtual void AddAll(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                DbContext.Entry<T>(entity);
                DbContext.Set<T>().Add(entity);
            }
        }

        
        //When you expect a model back (async)
        public async Task<IList<T>> ExecWithStoreProcedureAsync<T>(string query, params object[] parameters)
        {
            return await DbContext.Database.SqlQuery<T>(query, parameters).ToListAsync();
        }

        //When you expect a model back
        public IEnumerable<T> ExecWithStoreProcedure<T>(string query, params object[] parameters)
        {
            return DbContext.Database.SqlQuery<T>(query, parameters).ToList();
        }

        // Fire and forget (async)
        public async Task ExecuteWithStoreProcedureAsync(string query, params object[] parameters)
        {
            await DbContext.Database.ExecuteSqlCommandAsync(query, parameters);
        }

        // Fire and forget
        public void ExecuteWithStoreProcedure(string query, params object[] parameters)
        {
            DbContext.Database.ExecuteSqlCommand(query, parameters);
        }
    }
}
