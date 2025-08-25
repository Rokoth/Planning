using Contracts.Model.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class DataService<T, TDto> where T: DB.Context.Entity
    {
        protected IServiceProvider _serviceProvider;

        public DataService(IServiceProvider serviceProvider)
        {

        }

        protected async Task<TDto> ExecuteAsync(Func<DB.Repository.IRepository<T>, Task<TDto>> execute)
        {
            return await InternalExecute(execute);
        }

        protected async Task<PagedResult<TDto>> ExecuteListAsync(Func<DB.Repository.IRepository<T>, Task<PagedResult<TDto>>> execute)
        {
            return await InternalExecute(execute);
        }

        private async Task<TRes> InternalExecute<TRes>(Func<DB.Repository.IRepository<T>, Task<TRes>> execute)
        {
            try
            {
                var repo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<T>>();
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
