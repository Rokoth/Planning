using Contracts.Model.Common;
using Contracts.Model.Direction;
using Contracts.Model.User;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class UserHistoryDataService : DataGetService<DB.Context.UserHistory, UserHistory,
        UserHistoryFilter>
    {
        public UserHistoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Func<DB.Context.Filter<DB.Context.UserHistory>, CancellationToken, 
            Task<PagedResult<DB.Context.UserHistory>>> GetListFunc(DB.Repository.IRepository<DB.Context.UserHistory> repo)
        {
            return repo.GetAsyncDeleted;
        }

        protected override Expression<Func<DB.Context.UserHistory, bool>> GetFilter(UserHistoryFilter filter)
        {
            return s => (filter.Name == null || s.Name.Contains(filter.Name)) 
                && (filter.Id == null || s.Id == filter.Id);
        }
    }

    public class DirectionCategoryHistoryDataService : DataGetService<DB.Context.DirectionCategoryHistory, DirectionCategoryHistory,
        DirectionCategoryHistoryFilter>
    {
        public DirectionCategoryHistoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Expression<Func<DB.Context.DirectionCategoryHistory, bool>> GetFilter(DirectionCategoryHistoryFilter filter)
        {
            throw new NotImplementedException();
        }
    }

    public class DirectionHistoryDataService : DataGetService<DB.Context.DirectionHistory, DirectionHistory,
        DirectionHistoryFilter>
    {
        public DirectionHistoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Expression<Func<DB.Context.DirectionHistory, bool>> GetFilter(DirectionHistoryFilter filter)
        {
            throw new NotImplementedException();
        }
    }

    public class DirectionProjectHistoryDataService : DataGetService<DB.Context.DirectionProjectHistory, DirectionProjectHistory,
        DirectionProjectHistoryFilter>
    {
        public DirectionProjectHistoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Expression<Func<DB.Context.DirectionProjectHistory, bool>> GetFilter(DirectionProjectHistoryFilter filter)
        {
            throw new NotImplementedException();
        }
    }
}
