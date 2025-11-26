using Microsoft.Extensions.DependencyInjection;
using Planning.Contracts.Model;
using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public abstract class DataService<TEntity, Tdto, TFilter, TCreator, TUpdater> :
        DataGetService<TEntity, Tdto, TFilter>, IAddDataService<Tdto, TCreator>, IUpdateDataService<Tdto, TUpdater>, IDeleteDataService<Tdto>
          where TEntity : DB.Context.IEntity
          where TUpdater : Contracts.Model.IEntity
          where Tdto : Contracts.Model.Entity
          where TFilter : Contracts.Model.Filter<Tdto>
    {

        public DataService(IServiceProvider serviceProvider) : base(serviceProvider)
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

    public interface IGetDataService<Tdto, TFilter>
        where Tdto : Contracts.Model.Entity
        where TFilter : Contracts.Model.Filter<Tdto>
    {
        Task<Tdto> GetAsync(Guid id, CancellationToken token);
        Task<Contracts.Model.PagedResult<Tdto>> GetAsync(TFilter filter, CancellationToken token);
    }

    public interface IAddDataService<Tdto, TCreator> where Tdto : Contracts.Model.Entity
    {
        Task<Tdto> AddAsync(TCreator entity, CancellationToken token);
    }

    public interface IUpdateDataService<Tdto, TUpdater> where Tdto : Contracts.Model.Entity
    {       
        Task<Tdto> UpdateAsync(TUpdater entity, CancellationToken token);
    }

    public interface IDeleteDataService<Tdto> where Tdto : Contracts.Model.Entity
    {
        Task<Tdto> DeleteAsync(Guid id, CancellationToken token);
    }

    public static class DataServiceExtension
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            services.AddDataService<UserDataService, DB.Context.User, Contracts.Model.User,
                Contracts.Model.UserFilter, Contracts.Model.UserCreator, Contracts.Model.UserUpdater>();
            services.AddDataService<FormulaDataService, DB.Context.Formula, Contracts.Model.Formula,
                Contracts.Model.FormulaFilter, Contracts.Model.FormulaCreator, Contracts.Model.FormulaUpdater>();
            services.AddDataService<ProjectDataService, DB.Context.Project, Contracts.Model.Project,
                Contracts.Model.ProjectFilter, Contracts.Model.ProjectCreator, Contracts.Model.ProjectUpdater>();
            services.AddDataService<AdditionalTaskDataService, DB.Context.AdditionalTask, Contracts.Model.AdditionalTask,
                Contracts.Model.AdditionalTaskFilter, Contracts.Model.AdditionalTaskCreator, Contracts.Model.AdditionalTaskUpdater>();
            services.AddDataService<ScheduleDataService, DB.Context.Schedule, Contracts.Model.Schedule,
               Contracts.Model.ScheduleFilter, Contracts.Model.ScheduleCreator, Contracts.Model.ScheduleUpdater>();

            services.AddScoped<IGetDataService<Contracts.Model.UserHistory, Contracts.Model.UserHistoryFilter>, UserHistoryDataService>();
            services.AddScoped<IGetDataService<Contracts.Model.FormulaHistory, Contracts.Model.FormulaHistoryFilter>, FormulaHistoryDataService>();
            services.AddScoped<IGetDataService<Contracts.Model.ProjectHistory, Contracts.Model.ProjectHistoryFilter>, ProjectHistoryDataService>();
            services.AddScoped<IGetDataService<Contracts.Model.ScheduleHistory, Contracts.Model.ScheduleHistoryFilter>, ScheduleHistoryDataService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IIntegrationService, IntegrationService>();
            

            return services;
        }

        private static IServiceCollection AddDataService<TService, TEntity, Tdto, TFilter, TCreator, TUpdater>(this IServiceCollection services)
            where TEntity : DB.Context.Entity
            where TUpdater : Contracts.Model.IEntity
            where TService : DataService<TEntity, Tdto, TFilter, TCreator, TUpdater>
            where Tdto : Contracts.Model.Entity
            where TFilter : Contracts.Model.Filter<Tdto>
        {
            services.AddScoped<IGetDataService<Tdto, TFilter>, TService>();
            services.AddScoped<IAddDataService<Tdto, TCreator>, TService>();
            services.AddScoped<IUpdateDataService<Tdto, TUpdater>, TService>();
            services.AddScoped<IDeleteDataService<Tdto>, TService>();
            return services;
        }
    }

    [Serializable]
    internal class DataServiceException : Exception
    {
        public DataServiceException()
        {
        }

        public DataServiceException(string message) : base(message)
        {
        }

        public DataServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DataServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
