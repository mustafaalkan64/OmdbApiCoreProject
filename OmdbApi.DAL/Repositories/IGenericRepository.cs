using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<bool> Add(T entity);

        Task<List<T>> GetAll();

        Task<List<T>> GetAll(params Expression<Func<T, object>>[] includes);

        Task<List<T>> SearchBy(Expression<Func<T, bool>> searchBy, params Expression<Func<T, object>>[] includes);

        Task<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        Task<bool> Update(T entity);

        Task<bool> Delete(Expression<Func<T, bool>> identity, params Expression<Func<T, object>>[] includes);

        Task<bool> Delete(T entity);
        Task<bool> Any(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    }
}
