using Contracts.Model.Common;
using Contracts.Model.Project;
using Contracts.Model.Schedule;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class ProjectHistoryDataService : DataGetService<DB.Context.ProjectHistory, ProjectHistory,
        ProjectHistoryFilter>
    {
        public ProjectHistoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Func<DB.Context.Filter<DB.Context.ProjectHistory>, CancellationToken,
            Task<PagedResult<DB.Context.ProjectHistory>>> GetListFunc(DB.Repository.IRepository<DB.Context.ProjectHistory> repo)
        {
            return repo.GetAsyncDeleted;
        }

        protected override Expression<Func<DB.Context.ProjectHistory, bool>> GetFilter(ProjectHistoryFilter filter)
        {
            return s => (filter.Name == null || s.Name.Contains(filter.Name))
                && (filter.Id == null || s.Id == filter.Id) && s.UserId == filter.UserId;
        }
    }

    public class ScheduleHistoryDataService : DataGetService<DB.Context.ScheduleHistory, ScheduleHistory,
       ScheduleHistoryFilter>
    {
        public ScheduleHistoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Func<DB.Context.Filter<DB.Context.ScheduleHistory>, CancellationToken,
            Task<PagedResult<DB.Context.ScheduleHistory>>> GetListFunc(DB.Repository.IRepository<DB.Context.ScheduleHistory> repo)
        {
            return repo.GetAsyncDeleted;
        }

        protected override Expression<Func<DB.Context.ScheduleHistory, bool>> GetFilter(ScheduleHistoryFilter filter)
        {
            return s =>  (filter.Id == null || s.Id == filter.Id);
        }
    }
}
