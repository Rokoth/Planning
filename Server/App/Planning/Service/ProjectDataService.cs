using Microsoft.Extensions.DependencyInjection;
using Planning.Contracts.Model;
using Planning.DB.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class ProjectDataService : DataService<DB.Context.Project, Contracts.Model.Project,
       Contracts.Model.ProjectFilter, Contracts.Model.ProjectCreator, Contracts.Model.ProjectUpdater>
    {
        public ProjectDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Expression<Func<DB.Context.Project, bool>> GetFilter(Contracts.Model.ProjectFilter filter)
        {
            return s => s.UserId == filter.UserId && (filter.Name == null || s.Name.Contains(filter.Name)) && 
                        (filter.IsLeaf == null || s.IsLeaf == filter.IsLeaf) &&
                        (filter.LastUsedDateBegin == null || s.LastUsedDate >= filter.LastUsedDateBegin) &&
                        (filter.LastUsedDateEnd == null || s.LastUsedDate <= filter.LastUsedDateEnd) &&
                        (s.ParentId == filter.ParentId) &&
                        (filter.Path == null || s.Path.Contains(filter.Path));
        }

        protected override async Task PrepareBeforeAdd(DB.Repository.IRepository<DB.Context.Project> repository,
            Contracts.Model.ProjectCreator creator, CancellationToken token)
        {           
            var parent = await repository.GetAsync(new DB.Context.Filter<DB.Context.Project>()
            {
                Page = 0,
                Size = 10,
                Selector = s => s.Id == creator.ParentId && s.IsLeaf
            }, token);
            foreach (var item in parent.Data)
            {
                item.IsLeaf = false;
                await repository.UpdateAsync(item, false, token);
            }
        }

        protected override async Task PrepareBeforeUpdate(DB.Repository.IRepository<DB.Context.Project> repository,
            Contracts.Model.ProjectUpdater entity, CancellationToken token)
        {
            var parent = await repository.GetAsync(new DB.Context.Filter<DB.Context.Project>()
            {
                Page = 0,
                Size = 10,
                Selector = s => s.Id == entity.ParentId && s.IsLeaf
            }, token);
            foreach (var item in parent.Data)
            {
                item.IsLeaf = false;
                await repository.UpdateAsync(item, false, token);
            }
        }

        protected override async Task PrepareBeforeDelete(DB.Repository.IRepository<DB.Context.Project> repository,
            DB.Context.Project entity, CancellationToken token)
        {
            if (entity.ParentId.HasValue)
            {
                var parent = await repository.GetAsync(entity.ParentId.Value, token);
                var childs = await repository.GetAsync(new DB.Context.Filter<DB.Context.Project>()
                {
                    Page = 0,
                    Size = 10,
                    Selector = s => s.ParentId == parent.Id
                }, token);
                if (!childs.Data.Any())
                {
                    parent.IsLeaf = true;
                    await repository.UpdateAsync(parent, false, token);
                }
            }
        }

        protected override DB.Context.Project UpdateFillFields(Contracts.Model.ProjectUpdater entity, DB.Context.Project entry)
        {
            entry.Path = entity.Path;
            entry.Name = entity.Name;
            entry.ParentId = entity.ParentId;
            entry.Period = entity.Period;
            entry.Priority = entity.Priority;
            return entry;
        }

        protected override DB.Context.Project MapToEntityAdd(Contracts.Model.ProjectCreator creator)
        {
            var entity = base.MapToEntityAdd(creator);
            entity.LastUsedDate = DateTimeOffset.Now;
            return entity;
        }

        protected override async Task<Project> Enrich(Project entity, CancellationToken token)
        {
            var addTasksRepo = _serviceProvider.GetRequiredService<IRepository<DB.Context.AdditionalTask>>();
            var result = await base.Enrich(entity, token);
            var addTasks = await addTasksRepo.GetAsync(new DB.Context.Filter<DB.Context.AdditionalTask>()
            {
                Selector = s => s.IsDeleted == false && s.ProjectId == entity.Id
            }, token);
            result.AdditionalTasks = MapAdditionalTasks(addTasks);
            return result;
        }

        protected override async Task<IEnumerable<Project>> Enrich(IEnumerable<Project> entities, CancellationToken token)
        {
            var result = (await base.Enrich(entities, token)).ToList();
            var addTasksRepo = _serviceProvider.GetRequiredService<IRepository<DB.Context.AdditionalTask>>();

            foreach(var item in result)
            {
                var addTasks = await addTasksRepo.GetAsync(new DB.Context.Filter<DB.Context.AdditionalTask>()
                {
                    Selector = s => s.IsDeleted == false && s.ProjectId == item.Id
                }, token);
                item.AdditionalTasks = MapAdditionalTasks(addTasks);
            }

            return result;
        }

        private static System.Collections.Generic.List<AdditionalTask> MapAdditionalTasks(PagedResult<DB.Context.AdditionalTask> addTasks)
        {
            return addTasks.Data.Select(s => new AdditionalTask()
            {
                ConditionId = s.ConditionId,
                Id = s.Id,
                Name = s.Name,
                ProjectId = s.ProjectId,
                TaskData = s.TaskData,
                TypeId = s.TypeId,
                VersionDate = s.VersionDate
            }).ToList();
        }

        protected override string DefaultSort => "Name";

    }
}
