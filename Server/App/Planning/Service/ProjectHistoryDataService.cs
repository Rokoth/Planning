using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class ProjectHistoryDataService : DataGetService<DB.Context.ProjectHistory, Contract.Model.ProjectHistory,
        Contract.Model.ProjectHistoryFilter>
    {
        public ProjectHistoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Func<DB.Context.Filter<DB.Context.ProjectHistory>, CancellationToken,
            Task<Contract.Model.PagedResult<DB.Context.ProjectHistory>>> GetListFunc(DB.Repository.IRepository<DB.Context.ProjectHistory> repo)
        {
            return repo.GetAsyncDeleted;
        }

        protected override Expression<Func<DB.Context.ProjectHistory, bool>> GetFilter(Contract.Model.ProjectHistoryFilter filter)
        {
            return s => (filter.Name == null || s.Name.Contains(filter.Name))
                && (filter.Id == null || s.Id == filter.Id) && s.UserId == filter.UserId;
        }
    }
}
