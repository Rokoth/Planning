using Contracts.Model.Direction;
using Contracts.Model.User;
using System;
using System.Linq.Expressions;

namespace Planning.Service
{
    public class DirectionProjectDataService : DataService<DB.Context.DirectionProject, DirectionProject,
        DirectionProjectFilter, DirectionProjectCreator, DirectionProjectUpdater>
    {
        public DirectionProjectDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Id";

        protected override Expression<Func<DB.Context.DirectionProject, bool>> GetFilter(DirectionProjectFilter filter)
        {
            throw new NotImplementedException();
        }

        protected override DB.Context.DirectionProject UpdateFillFields(DirectionProjectUpdater entity, DB.Context.DirectionProject entry)
        {
            throw new NotImplementedException();
        }
    }
}
