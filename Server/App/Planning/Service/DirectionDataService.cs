using Contracts.Model.Direction;
using Contracts.Model.User;
using System;
using System.Linq.Expressions;

namespace Planning.Service
{
    public class DirectionDataService : DataService<DB.Context.Direction, Direction,
        DirectionFilter, DirectionCreator, DirectionUpdater>
    {
        public DirectionDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Expression<Func<DB.Context.Direction, bool>> GetFilter(DirectionFilter filter)
        {
            throw new NotImplementedException();
        }

        protected override DB.Context.Direction UpdateFillFields(DirectionUpdater entity, DB.Context.Direction entry)
        {
            throw new NotImplementedException();
        }
    }
}
