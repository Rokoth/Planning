using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class ScheduleDataService : DataService<DB.Context.Schedule, Contracts.Model.Schedule,
       Contracts.Model.ScheduleFilter, Contracts.Model.ScheduleCreator, Contracts.Model.ScheduleUpdater>
    {
        public ScheduleDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Expression<Func<DB.Context.Schedule, bool>> GetFilter(Contracts.Model.ScheduleFilter filter)
        {
            

            return s => s.UserId == filter.UserId && 
                (filter.ProjectId == null || s.ProjectId == filter.ProjectId) &&                
                (filter.FromDate == null || s.BeginDate >= filter.FromDate) &&
                (filter.ToDate == null || s.BeginDate <= filter.ToDate) &&
                (filter.OnlyActive == null || !filter.OnlyActive.Value || !s.IsClosed);
        }
               
        protected override DB.Context.Schedule UpdateFillFields(Contracts.Model.ScheduleUpdater entity, DB.Context.Schedule entry)
        {
            entry.BeginDate = entity.BeginDate;
            entry.ProjectId = entity.ProjectId;          
            return entry;
        }

        protected override async Task<Contracts.Model.Schedule> Enrich(Contracts.Model.Schedule entity, CancellationToken token)
        {
            var _projectRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Project>>();
            var _addTaskRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.AdditionalTask>>();
            var fullProj = await GetFullProjectName(_projectRepo, entity.ProjectId);
            entity.Project = fullProj.Name;
            entity.ProjectPath = fullProj.Path;
            entity.AdditionalTasks = await GetAdditionalTasks(entity.ProjectId, _addTaskRepo, token);
            return entity;
        }

        private async Task<string> GetAdditionalTasks(Guid projectId, DB.Repository.IRepository<DB.Context.AdditionalTask> repo, CancellationToken token)
        {
            var addTasks = await repo.GetAsync(new DB.Context.Filter<DB.Context.AdditionalTask>()
            {
                Selector = s => s.IsDeleted == false && s.ProjectId == projectId
            }, token);

            return $"Привязано {addTasks.Data.Count()} допонительных дествий: {string.Join(", ", addTasks.Data.Select(s => s.Name))}";
        }

        protected async Task<ProjectTemp> GetFullProjectName(DB.Repository.IRepository<DB.Context.Project> repo, Guid projectId)
        {            
            var project = await repo.GetAsync(projectId, CancellationToken.None);
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
                var parentProject = await GetFullProjectName(repo, project.ParentId.Value);
                result.Name = parentProject.Name + "/" + result.Name;
                result.Path = parentProject.Path + "\\" + result.Path;
            }
            return result;
        }

        /// <summary>
        /// function for enrichment data item
        /// </summary>
        protected override async Task<IEnumerable<Contracts.Model.Schedule>> Enrich(IEnumerable<Contracts.Model.Schedule> entities, CancellationToken token)
        {
            List<Contracts.Model.Schedule> result = new List<Contracts.Model.Schedule>();
            if (entities.Any())
            {
                var userId = entities.First().UserId;
                var _projectRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Project>>();
                var _addTaskRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.AdditionalTask>>();
                var allProjects = await _projectRepo.GetAsync(new DB.Context.Filter<DB.Context.Project>() {
                    Selector = s => s.UserId == userId
                }, token);
                foreach (var item in entities)
                {
                    var fullProj = GetFullProjectName(allProjects.Data, item.ProjectId);
                    item.Project = fullProj.Name;
                    item.ProjectPath = fullProj.Path;
                    item.AdditionalTasks = await GetAdditionalTasks(item.ProjectId, _addTaskRepo, token);
                    result.Add(item);
                }               
            }
            return result;
        }

        protected ProjectTemp GetFullProjectName(IEnumerable<DB.Context.Project> projects, Guid projectId)
        {
            var project = projects.FirstOrDefault(s=>s.Id == projectId);
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

        protected override string DefaultSort => "BeginDate";

    }
}
