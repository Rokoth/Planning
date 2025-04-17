using Contracts.Model.Common;
using Contracts.Model.Direction;
using Contracts.Model.Formula;
using Contracts.Model.Project;
using Contracts.Model.Schedule;
using Contracts.Model.User;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public abstract class DataService<TEntity, Tdto, TFilter, TCreator, TUpdater> :
        DataGetService<TEntity, Tdto, TFilter>, IAddDataService<Tdto, TCreator>, IUpdateDataService<Tdto, TUpdater>, IDeleteDataService<Tdto>
          where TEntity : DB.Context.IEntity
          where TUpdater : IEntity
          where Tdto : Entity
          where TFilter : Filter<Tdto>
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
        where Tdto : Entity
        where TFilter : Filter<Tdto>
    {
        Task<Tdto> GetAsync(Guid id, CancellationToken token);
        Task<PagedResult<Tdto>> GetAsync(TFilter filter, CancellationToken token);
    }

    public interface IAddDataService<Tdto, TCreator> where Tdto : Entity
    {
        Task<Tdto> AddAsync(TCreator entity, CancellationToken token);
    }

    public interface IUpdateDataService<Tdto, TUpdater> where Tdto : Entity
    {
        Task<Tdto> UpdateAsync(TUpdater entity, CancellationToken token);
    }

    public interface IDeleteDataService<Tdto> where Tdto : Entity
    {
        Task<Tdto> DeleteAsync(Guid id, CancellationToken token);
    }

    public static class DataServiceExtension
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            services.AddDataService<UserDataService, DB.Context.User, User,
                UserFilter, UserCreator, UserUpdater>();
            services.AddDataService<FormulaDataService, DB.Context.Formula, Formula,
                FormulaFilter, FormulaCreator, FormulaUpdater>();
            services.AddDataService<ProjectDataService, DB.Context.Project, Project,
                ProjectFilter, ProjectCreator, ProjectUpdater>();
            services.AddDataService<ScheduleDataService, DB.Context.Schedule, Schedule,
               ScheduleFilter, ScheduleCreator, ScheduleUpdater>();

            services.AddDataService<DirectionCategoryDataService, DB.Context.DirectionCategory, DirectionCategory,
               DirectionCategoryFilter, DirectionCategoryCreator, DirectionCategoryUpdater>();
            services.AddDataService<DirectionDataService, DB.Context.Direction, Direction,
               DirectionFilter, DirectionCreator, DirectionUpdater>();
            services.AddDataService<DirectionProjectDataService, DB.Context.DirectionProject, DirectionProject,
               DirectionProjectFilter, DirectionProjectCreator, DirectionProjectUpdater>();

            services.AddScoped<IGetDataService<UserHistory, UserHistoryFilter>, UserHistoryDataService>();
            services.AddScoped<IGetDataService<FormulaHistory, FormulaHistoryFilter>, FormulaHistoryDataService>();
            services.AddScoped<IGetDataService<ProjectHistory, ProjectHistoryFilter>, ProjectHistoryDataService>();
            services.AddScoped<IGetDataService<ScheduleHistory, ScheduleHistoryFilter>, ScheduleHistoryDataService>();

            services.AddScoped<IGetDataService<DirectionCategoryHistory, DirectionCategoryHistoryFilter>, DirectionCategoryHistoryDataService>();
            services.AddScoped<IGetDataService<DirectionHistory, DirectionHistoryFilter>, DirectionHistoryDataService>();
            services.AddScoped<IGetDataService<DirectionProjectHistory, DirectionProjectHistoryFilter>, DirectionProjectHistoryDataService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

        private static IServiceCollection AddDataService<TService, TEntity, Tdto, TFilter, TCreator, TUpdater>(this IServiceCollection services)
            where TEntity : DB.Context.Entity
            where TUpdater : IEntity
            where TService : DataService<TEntity, Tdto, TFilter, TCreator, TUpdater>
            where Tdto : Entity
            where TFilter : Filter<Tdto>
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
