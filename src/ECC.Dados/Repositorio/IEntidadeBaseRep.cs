
using ECC.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ECC.Dados.Repositorio
{
    public interface IEntidadeBaseRep<T> where T : class, IEntidadeBase, new()
    {
        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> All { get; }
        IQueryable<T> GetAll();
        T GetSingle(int id);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(T entity);
        void DeleteAll(IEnumerable<T> entities);

        void AddAll(IEnumerable<T> entities);
        void Edit(T entity);
        Task<IList<T>> ExecWithStoreProcedureAsync<T>(string query, params object[] parameters);
        IEnumerable<T> ExecWithStoreProcedure<T>(string query, params object[] parameters);
        Task ExecuteWithStoreProcedureAsync(string query, params object[] parameters);
        void ExecuteWithStoreProcedure(string query, params object[] parameters);
    }
}
