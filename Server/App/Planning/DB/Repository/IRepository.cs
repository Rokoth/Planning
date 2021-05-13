using Planning.DB.Context;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Planning.DB.Repository
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task<TEntity> Add(TEntity entity, bool withSaving);
        Task<TEntity> Delete(TEntity entity, bool withSaving);
        Task<TEntity> Get(Guid id);
        Task<IEnumerable<TEntity>> GetDeletedList(Expression<Func<TEntity, bool>> filter);
        Task<IEnumerable<TEntity>> GetList(Expression<Func<TEntity, bool>> filter);
        Task<TEntity> Update(TEntity entity, bool withSaving);
    }
}