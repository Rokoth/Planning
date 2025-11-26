using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class AdditionalTaskDataService : DataService<DB.Context.AdditionalTask, Contracts.Model.AdditionalTask,
       Contracts.Model.AdditionalTaskFilter, Contracts.Model.AdditionalTaskCreator, Contracts.Model.AdditionalTaskUpdater>
    {
        public AdditionalTaskDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Expression<Func<DB.Context.AdditionalTask, bool>> GetFilter(Contracts.Model.AdditionalTaskFilter filter)
        {
            return s => (filter.Name == null || s.Name.Contains(filter.Name)) &&                        
                        (s.ProjectId == filter.ProjectId);
        }


        protected override DB.Context.AdditionalTask UpdateFillFields(Contracts.Model.AdditionalTaskUpdater entity, DB.Context.AdditionalTask entry)
        {           
            entry.Name = entity.Name;
            entry.ProjectId = entity.ProjectId;
            entry.TaskData = entity.TaskData;
            entry.ConditionId = entity.ConditionId;
            entry.TypeId = entity.TypeId;
            return entry;
        }

        protected override string DefaultSort => "Name";

    }
}
