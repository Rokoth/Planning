using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.DB.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Planning.DB.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly DbPgContext mainContext;
        private readonly ILogger<Repository<TEntity>> logger;

        public Repository(IServiceProvider serviceProvider)
        {
            mainContext = serviceProvider.GetRequiredService<DbPgContext>();
            logger = serviceProvider.GetRequiredService<ILogger<Repository<TEntity>>>();
        }

        public async Task<IEnumerable<TEntity>> GetList(Expression<Func<TEntity, bool>> filter)
        {
            return await CallFunc(async () =>
            {
                return await mainContext.Set<TEntity>().Where(s => !s.IsDeleted).Where(filter).ToListAsync();
            }, "GetList");
        }

        public async Task<IEnumerable<TEntity>> GetDeletedList(Expression<Func<TEntity, bool>> filter)
        {
            return await CallFunc(async () =>
            {
                return await mainContext.Set<TEntity>().Where(s => s.IsDeleted).Where(filter).ToListAsync();
            }, "GetDeletedList");
        }

        public async Task<TEntity> Get(Guid id)
        {
            return await CallFunc(async () =>
            {
                return await mainContext.Set<TEntity>().Where(s => s.Id == id).FirstOrDefaultAsync();
            }, "Get");
        }

        public async Task<TEntity> Add(TEntity entity, bool withSaving)
        {
            return await CallFunc(async () =>
            {
                var result = mainContext.Add(entity).Entity;
                if (withSaving) await mainContext.SaveChangesAsync();
                return result;
            }, "Add");
        }

        public async Task<TEntity> Update(TEntity entity, bool withSaving)
        {
            return await CallFunc(async () =>
            {
                entity.VersionDate = DateTimeOffset.Now;
                var result = mainContext.Update(entity).Entity;
                if (withSaving) await mainContext.SaveChangesAsync();
                return result;
            }, "Update");
        }

        public async Task<TEntity> Delete(TEntity entity, bool withSaving)
        {
            return await CallFunc(async () =>
            {
                entity.IsDeleted = true;
                entity.VersionDate = DateTimeOffset.Now;
                var result = mainContext.Update(entity).Entity;
                if (withSaving) await mainContext.SaveChangesAsync();
                return result;
            }, "Delete");
        }

        private async Task<T> CallFunc<T>(Func<Task<T>> func, string method)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception throws in {method}: {ex.Message}\r\nStackTrace: {ex.StackTrace}");
                throw new RepositoryException($"Exception throws in {method}: {ex.Message}");
            }
        }

    }

    [Serializable]
    internal class RepositoryException : Exception
    {
        public RepositoryException()
        {
        }

        public RepositoryException(string message) : base(message)
        {
        }

        public RepositoryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RepositoryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
