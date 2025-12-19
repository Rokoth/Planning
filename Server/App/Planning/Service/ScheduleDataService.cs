using Contracts.Model.Common;
using Contracts.Model.Schedule;
using Microsoft.Extensions.DependencyInjection;
using Planning.DB.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class ScheduleDataService : IScheduleDataService
    {
        private readonly IRepository<DB.Context.Schedule> _sheduleRepository;
        private readonly IRepository<DB.Context.Project> _projectRepo;
        private readonly IProjectSelectService _projectSelectService;

        public ScheduleDataService(IRepository<DB.Context.Schedule> sheduleRepository,
            IRepository<DB.Context.Project> projectRepo, 
            IProjectSelectService projectSelectService)
        {
            _sheduleRepository = sheduleRepository;
            _projectRepo = projectRepo;
            _projectSelectService = projectSelectService;
        }

        public async Task<IEnumerable<Schedule>> GetCurrentScheduleAsync(Guid userId, CancellationToken token)
        {
            var data = (await _sheduleRepository.GetAsync(new DB.Context.Filter<DB.Context.Schedule>
            {                
                Selector = s => s.UserId == userId && s.IsRunning
            }, token)).Data.FirstOrDefault();

            var result = (await Map(new List<DB.Context.Schedule>() { data }, token)).ToList();
            var nextSchedules = await _projectSelectService.GetNextShedules(userId, 9, data.EndDate, token);
            nextSchedules = await Enrich(nextSchedules, token);
            result.AddRange(nextSchedules);
            return result.OrderBy(s => s.BeginDate);
        }

        public async Task<PagedResult<Schedule>> GetListAsync(ScheduleFilter filter, CancellationToken token)
        {
            var result = await _sheduleRepository.GetAsync(new DB.Context.Filter<DB.Context.Schedule>
            {
                Size = filter.Size,
                Page = filter.Page,
                Sort = filter.Sort ?? "BeginDate",
                Selector = GetFilter(filter)
            }, token);

            return new PagedResult<Schedule>(await Map(result, token), result.PageCount);
        }

        private async Task<IEnumerable<Schedule>> Map(List<DB.Context.Schedule> result, CancellationToken token)
        {
            var prepare = result.Select(s => new Schedule()
            {
                BeginDate = s.BeginDate,
                EndDate = s.EndDate,
                Id = s.Id,
                IsRunning = s.IsRunning,
                ProjectId = s.ProjectId,
                UserId = s.UserId,
                VersionDate = s.VersionDate
            });
            prepare = await Enrich(prepare, token);
            return prepare;
        }

        private static Expression<Func<DB.Context.Schedule, bool>> GetFilter(ScheduleFilter filter)
        {
            return s => s.UserId == filter.UserId &&
                (filter.ProjectId == null || s.ProjectId == filter.ProjectId) &&
                (filter.FromDate == null || s.BeginDate >= filter.FromDate) &&
                (filter.ToDate == null || s.BeginDate <= filter.ToDate) &&
                (filter.OnlyActive == null || !filter.OnlyActive.Value || !s.IsClosed);
        }


        private async Task<Schedule> Enrich(Schedule entity, CancellationToken token)
        {
            var fullProj = await GetFullProjectName(entity.ProjectId);
            entity.Project = fullProj.Name;
            entity.ProjectPath = fullProj.Path;
            return entity;
        }

        private async Task<ProjectTemp> GetFullProjectName(Guid projectId)
        {
            var project = await _projectRepo.GetAsync(projectId, CancellationToken.None);
            if (project == null)
            {
                return new ProjectTemp()
                {
                    Name = "Удалён",
                    Path = "Удалён"
                };
            }
            var result = new ProjectTemp()
            {
                Name = project.Name,
                Path = project.Path
            };
            if (project.ParentId != null)
            {
                var parentProject = await GetFullProjectName(project.ParentId.Value);
                result.Name = parentProject.Name + "/" + result.Name;
                result.Path = parentProject.Path + "\\" + result.Path;
            }
            return result;
        }

        private static ProjectTemp GetFullProjectName(IEnumerable<DB.Context.Project> projects, Guid projectId)
        {
            var project = projects.FirstOrDefault(s => s.Id == projectId);
            if (project == null)
            {
                return new ProjectTemp()
                {
                    Name = "Удалён",
                    Path = "Удалён"
                };
            }
            var result = new ProjectTemp()
            {
                Name = project.Name,
                Path = project.Path
            };
            if (project.ParentId != null)
            {
                var parentProject = GetFullProjectName(projects, project.ParentId.Value);
                result.Name = parentProject.Name + "/" + result.Name;
                result.Path = parentProject.Path + "\\" + result.Path;
            }
            return result;
        }

        /// <summary>
        /// function for enrichment data item
        /// </summary>
        private async Task<IEnumerable<Schedule>> Enrich(IEnumerable<Schedule> entities, CancellationToken token)
        {
            List<Schedule> result = new();
            if (entities.Any())
            {
                var userId = entities.First().UserId;
                var allProjects = await _projectRepo.GetAsync(new DB.Context.Filter<DB.Context.Project>()
                {
                    Selector = s => s.UserId == userId
                }, token);
                foreach (var item in entities)
                {
                    var fullProj = GetFullProjectName(allProjects.Data, item.ProjectId);
                    item.Project = fullProj.Name;
                    item.ProjectPath = fullProj.Path;
                    result.Add(item);
                }
            }
            return result;
        }



    }
}
