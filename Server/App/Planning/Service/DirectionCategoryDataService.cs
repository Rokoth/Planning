using Contracts.Model.Direction;
using Contracts.Model.User;
using System;
using System.Linq.Expressions;

namespace Planning.Service
{
    public class DirectionCategoryDataService : DataService<DB.Context.DirectionCategory, DirectionCategory,
        DirectionCategoryFilter, DirectionCategoryCreator, DirectionCategoryUpdater>
    {
        public DirectionCategoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
               
        protected override string DefaultSort => "Name";

        protected override Expression<Func<DB.Context.DirectionCategory, bool>> GetFilter(DirectionCategoryFilter filter)
        {
            throw new NotImplementedException();
        }

        protected override DB.Context.DirectionCategory UpdateFillFields(DirectionCategoryUpdater entity, DB.Context.DirectionCategory entry)
        {
            throw new NotImplementedException();
        }
    }
}
