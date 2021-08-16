
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{    
    public abstract class DataService<TEntity, Tdto, TFilter, TCreator, TUpdater> :
        DataGetService<TEntity, Tdto, TFilter>, IAddDataService<Tdto, TCreator>, IUpdateDataService<Tdto, TUpdater>, IDeleteDataService<Tdto>
          where TEntity : DB.Context.IEntity
          where TUpdater : Contract.Model.IEntity
          where Tdto : Contract.Model.Entity
          where TFilter : Contract.Model.Filter<Tdto>
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
                var prepare = _mapper.Map<Tdto>(entity);
                prepare = await Enrich(prepare, token);
                return prepare;
            });
        }
    }

    public interface IGetDataService<Tdto, TFilter>
        where Tdto : Contract.Model.Entity
        where TFilter : Contract.Model.Filter<Tdto>
    {
        Task<Tdto> GetAsync(Guid id, CancellationToken token);
        Task<Contract.Model.PagedResult<Tdto>> GetAsync(TFilter filter, CancellationToken token);
    }

    public interface IAddDataService<Tdto, TCreator> where Tdto : Contract.Model.Entity
    {
        Task<Tdto> AddAsync(TCreator entity, CancellationToken token);
    }

    public interface IUpdateDataService<Tdto, TUpdater> where Tdto : Contract.Model.Entity
    {
        Task<Tdto> UpdateAsync(TUpdater entity, CancellationToken token);
    }

    public interface IDeleteDataService<Tdto> where Tdto : Contract.Model.Entity
    {
        Task<Tdto> DeleteAsync(Guid id, CancellationToken token);
    }

    public static class DataServiceExtension
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            services.AddDataService<UserDataService, DB.Context.User, Contract.Model.User,
                Contract.Model.UserFilter, Contract.Model.UserCreator, Contract.Model.UserUpdater>();
            services.AddDataService<FormulaDataService, DB.Context.Formula, Contract.Model.Formula,
                Contract.Model.FormulaFilter, Contract.Model.FormulaCreator, Contract.Model.FormulaUpdater>();
            services.AddDataService<ProjectDataService, DB.Context.Project, Contract.Model.Project,
                Contract.Model.ProjectFilter, Contract.Model.ProjectCreator, Contract.Model.ProjectUpdater>();

            services.AddScoped<IGetDataService<Contract.Model.UserHistory, Contract.Model.UserHistoryFilter>, UserHistoryDataService>();
            services.AddScoped<IGetDataService<Contract.Model.FormulaHistory, Contract.Model.FormulaHistoryFilter>, FormulaHistoryDataService>();
            services.AddScoped<IGetDataService<Contract.Model.ProjectHistory, Contract.Model.ProjectHistoryFilter>, ProjectHistoryDataService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

        private static IServiceCollection AddDataService<TService, TEntity, Tdto, TFilter, TCreator, TUpdater>(this IServiceCollection services)
            where TEntity : DB.Context.Entity
            where TUpdater : Contract.Model.IEntity
            where TService : DataService<TEntity, Tdto, TFilter, TCreator, TUpdater>
            where Tdto : Contract.Model.Entity
            where TFilter : Contract.Model.Filter<Tdto>
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

    public abstract class DataGetService<TEntity, Tdto, TFilter> :
        IGetDataService<Tdto, TFilter>
        where TEntity : DB.Context.IEntity
        where Tdto : Contract.Model.Entity
        where TFilter : Contract.Model.Filter<Tdto>
    {
        protected IServiceProvider _serviceProvider;
        protected IMapper _mapper;

        protected abstract string DefaultSort { get; }

        /// <summary>
        /// function for modify client filter to db filter
        /// </summary>
        protected abstract Expression<Func<TEntity, bool>> GetFilter(TFilter filter);

        protected virtual Func<DB.Context.Filter<TEntity>, CancellationToken, Task<Contract.Model.PagedResult<TEntity>>> GetListFunc(DB.Repository.IRepository<TEntity> repo)
        {
            return repo.GetAsync;
        }

        /// <summary>
        /// function for enrichment data item
        /// </summary>
        protected virtual async Task<Tdto> Enrich(Tdto entity, CancellationToken token)
        {
            return entity;
        }

        /// <summary>
        /// function for enrichment data item
        /// </summary>
        protected virtual async Task<IEnumerable<Tdto>> Enrich(IEnumerable<Tdto> entities, CancellationToken token)
        {
            return entities;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DataGetService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _mapper = _serviceProvider.GetRequiredService<IMapper>();
        }

        /// <summary>
        /// Get items method
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Contract.Model.PagedResult<Tdto>> GetAsync(TFilter filter, CancellationToken token)
        {
            return await ExecuteAsync(async (repo) =>
            {
                string sort = filter.Sort;
                if (string.IsNullOrEmpty(sort))
                {
                    sort = DefaultSort;
                }

                var result = await GetListFunc(repo)(new DB.Context.Filter<TEntity>
                {
                    Size = filter.Size,
                    Page = filter.Page,
                    Sort = sort,
                    Selector = GetFilter(filter)
                }, token);
                var prepare = result.Data.Select(s => _mapper.Map<Tdto>(s));
                prepare = await Enrich(prepare, token);
                return new Contract.Model.PagedResult<Tdto>(prepare, result.PageCount);
            });
        }

        /// <summary>
        /// get item method
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Tdto> GetAsync(Guid id, CancellationToken token)
        {
            return await ExecuteAsync(async (repo) =>
            {
                var result = await repo.GetAsync(id, token);
                var prepare = _mapper.Map<Tdto>(result);
                prepare = await Enrich(prepare, token);
                return prepare;
            });
        }

        /// <summary>
        /// execution wrapper
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="execute"></param>
        /// <returns></returns>
        protected async Task<T> ExecuteAsync<T>(Func<DB.Repository.IRepository<TEntity>, Task<T>> execute)
        {
            try
            {
                var repo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<TEntity>>();
                return await execute(repo);
            }
            catch (DataServiceException)
            {
                throw;
            }
            catch (DB.Repository.RepositoryException ex)
            {
                throw new DataServiceException(ex.Message);
            }
        }
    }
}
