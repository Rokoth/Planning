using Contracts.Model.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public abstract class OldDataService<TEntity, Tdto, TFilter, TCreator, TUpdater> :
        DataGetService<TEntity, Tdto, TFilter>, IAddDataService<Tdto, TCreator>, IUpdateDataService<Tdto, TUpdater>, IDeleteDataService<Tdto>
          where TEntity : DB.Context.IEntity
          where TUpdater : IEntity
          where Tdto : Entity
          where TFilter : Filter<Tdto>
    {

        public OldDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected virtual TEntity MapToEntityAdd(TCreator creator)
        {
            var result = _mapper.Map<TEntity>(creator);
            result.Id = Guid.NewGuid();
            result.IsDeleted = false;
            result.VersionDate = DateTimeOffset.Now;
            return result;
        }

        protected virtual async Task PrepareBeforeAdd(DB.Repository.IRepository<TEntity> repository, TCreator creator, CancellationToken token)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task PrepareBeforeUpdate(DB.Repository.IRepository<TEntity> repository, TUpdater entity, CancellationToken token)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task PrepareBeforeDelete(DB.Repository.IRepository<TEntity> repository, TEntity entity, CancellationToken token)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task ActionAfterAdd(DB.Repository.IRepository<TEntity> repository, TCreator creator, TEntity entity, CancellationToken token)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task ActionAfterUpdate(DB.Repository.IRepository<TEntity> repository, TUpdater updater, TEntity entity, CancellationToken token)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task ActionAfterDelete(DB.Repository.IRepository<TEntity> repository, TEntity entity, CancellationToken token)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// add item method
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Tdto> AddAsync(TCreator creator, CancellationToken token)
        {
            return await ExecuteAsync(async (repo) =>
            {
                var entity = MapToEntityAdd(creator);
                await PrepareBeforeAdd(repo, creator, token);
                var result = await repo.AddAsync(entity, false, token);
                await ActionAfterAdd(repo, creator, result, token);
                await repo.SaveChangesAsync();
                var prepare = _mapper.Map<Tdto>(result);
                prepare = await Enrich(prepare, token);
                return prepare;
            });
        }

        protected abstract TEntity UpdateFillFields(TUpdater entity, TEntity entry);

        public async Task<Tdto> UpdateAsync(TUpdater entity, CancellationToken token)
        {
            return await ExecuteAsync(async (repo) =>
            {
                var entry = await repo.GetAsync(entity.Id, token);
                entry = UpdateFillFields(entity, entry);
                await PrepareBeforeUpdate(repo, entity, token);
                TEntity result = await repo.UpdateAsync(entry, false, token);
                await ActionAfterUpdate(repo, entity, result, token);
                await repo.SaveChangesAsync();
                var prepare = _mapper.Map<Tdto>(result);
                prepare = await Enrich(prepare, token);
                return prepare;
            });
        }

        public async Task<Tdto> DeleteAsync(Guid id, CancellationToken token)
        {
            return await ExecuteAsync(async (repo) =>
            {
                var entity = await repo.GetAsync(id, token);
                if (entity == null) throw new DataServiceException($"Entity with id = {id} not found in DB");
                await PrepareBeforeDelete(repo, entity, token);
                entity = await repo.DeleteAsync(entity, false, token);
                await ActionAfterDelete(repo, entity, token);
                await repo.SaveChangesAsync();
                return _mapper.Map<Tdto>(entity);                
            });
        }
    }
}
