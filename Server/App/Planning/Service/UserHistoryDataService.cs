using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class UserHistoryDataService : DataGetService<DB.Context.UserHistory, Contracts.Model.UserHistory,
        Contracts.Model.UserHistoryFilter>
    {
        public UserHistoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Func<DB.Context.Filter<DB.Context.UserHistory>, CancellationToken, 
            Task<Contracts.Model.PagedResult<DB.Context.UserHistory>>> GetListFunc(DB.Repository.IRepository<DB.Context.UserHistory> repo)
        {
            return repo.GetAsyncDeleted;
        }

        protected override Expression<Func<DB.Context.UserHistory, bool>> GetFilter(Contracts.Model.UserHistoryFilter filter)
        {
            return s => (filter.Name == null || s.Name.Contains(filter.Name)) 
                && (filter.Id == null || s.Id == filter.Id);
        }
    }
}
