using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class FormulaDataService : DataService<DB.Context.Formula, Contract.Model.Formula,
       Contract.Model.FormulaFilter, Contract.Model.FormulaCreator, Contract.Model.FormulaUpdater>
    {
        public FormulaDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Expression<Func<DB.Context.Formula, bool>> GetFilter(Contract.Model.FormulaFilter filter)
        {
            return s => (filter.Name == null || s.Name.Contains(filter.Name)) 
                     && (filter.IsDefault == null || s.IsDefault == filter.IsDefault);
        }

        protected override async Task PrepareBeforeAdd(DB.Repository.IRepository<DB.Context.Formula> repository, 
            Contract.Model.FormulaCreator creator, CancellationToken token)
        {
            if (creator.IsDefault)
            {
                var currentDefaults = await repository.GetAsync(new DB.Context.Filter<DB.Context.Formula>()
                {
                    Page = 0,
                    Size = 10,
                    Selector = s => s.IsDefault
                }, token);
                foreach (var item in currentDefaults.Data)
                {
                    item.IsDefault = false;
                    await repository.UpdateAsync(item, false,  token);
                }
            }
        }

        protected override async Task PrepareBeforeUpdate(DB.Repository.IRepository<DB.Context.Formula> repository, 
            Contract.Model.FormulaUpdater entity, CancellationToken token)
        {
            if (entity.IsDefault)
            {
                var currentDefaults = await repository.GetAsync(new DB.Context.Filter<DB.Context.Formula>()
                {
                    Page = 0,
                    Size = 10,
                    Selector = s => s.IsDefault && s.Id != entity.Id
                }, token);
                foreach (var item in currentDefaults.Data)
                {
                    item.IsDefault = false;
                    await repository.UpdateAsync(item, false, token);
                }
            }
        }

        protected override async Task PrepareBeforeDelete(DB.Repository.IRepository<DB.Context.Formula> repository, 
            DB.Context.Formula entity, CancellationToken token)
        {
            if (entity.IsDefault)
            {
                var currentDefault = (await repository.GetAsync(new DB.Context.Filter<DB.Context.Formula>()
                {
                    Page = 0,
                    Size = 10,
                    Selector = s => true
                }, token)).Data.FirstOrDefault();
                currentDefault.IsDefault = true;
                await repository.UpdateAsync(currentDefault, false, token);
            }
        }

        protected override DB.Context.Formula UpdateFillFields(Contract.Model.FormulaUpdater entity, DB.Context.Formula entry)
        {
            entry.Text = entity.Text;
            entry.Name = entity.Name;
            entry.IsDefault = entity.IsDefault;
            return entry;
        }

        protected override string DefaultSort => "Name";

    }

    public class ScheduleDataService : DataService<DB.Context.Schedule, Contract.Model.Schedule,
       Contract.Model.ScheduleFilter, Contract.Model.ScheduleCreator, Contract.Model.ScheduleUpdater>
    {
        public ScheduleDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Expression<Func<DB.Context.Schedule, bool>> GetFilter(Contract.Model.ScheduleFilter filter)
        {
            

            return s => s.UserId == filter.UserId && 
                (filter.ProjectId == null || s.ProjectId == filter.ProjectId) &&                
                (filter.FromDate == null || s.BeginDate >= filter.FromDate) &&
                (filter.ToDate == null || s.BeginDate <= filter.ToDate) &&
                (filter.OnlyActive == null || !filter.OnlyActive.Value || !s.IsClosed);
        }
               
        protected override DB.Context.Schedule UpdateFillFields(Contract.Model.ScheduleUpdater entity, DB.Context.Schedule entry)
        {
            entry.BeginDate = entity.BeginDate;
            entry.ProjectId = entity.ProjectId;          
            return entry;
        }

        protected override async Task<Contract.Model.Schedule> Enrich(Contract.Model.Schedule entity, CancellationToken token)
        {
            var _projectRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Project>>();
            var fullProj = await GetFullProjectName(_projectRepo, entity.ProjectId);
            entity.Project = fullProj.Name;
            entity.ProjectPath = fullProj.Path;
            return entity;
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
        protected override async Task<IEnumerable<Contract.Model.Schedule>> Enrich(IEnumerable<Contract.Model.Schedule> entities, CancellationToken token)
        {
            List<Contract.Model.Schedule> result = new List<Contract.Model.Schedule>();
            if (entities.Any())
            {
                var userId = entities.First().UserId;
                var _projectRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Project>>();
                var allProjects = await _projectRepo.GetAsync(new DB.Context.Filter<DB.Context.Project>() {
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

    public class ProjectTemp
    { 
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
