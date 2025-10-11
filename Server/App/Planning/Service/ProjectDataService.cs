using Contracts.Model.Common;
using Contracts.Model.Project;
using Contracts.Model.Schedule;
using Microsoft.Extensions.DependencyInjection;
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
    public class ProjectDataService : IProjectDataService
    {
        private readonly IRepository<DB.Context.Project> _repository;
        private readonly IRepository<DB.Context.UserSettings> _settingsRepository;

        public ProjectDataService(IRepository<DB.Context.Project> repository,
            IRepository<DB.Context.UserSettings> settingsRepository)
        {
            _repository = repository;
            _settingsRepository = settingsRepository;
        }

        public async Task<PagedResult<Project>> GetListAsync(ProjectFilter filter, CancellationToken token)
        {
            var result = await _repository.GetAsync(new DB.Context.Filter<DB.Context.Project>
            {
                Size = filter.Size,
                Page = filter.Page,
                Sort = filter.Sort ?? "BeginDate",
                Selector = GetFilter(filter)
            }, token);

            return new PagedResult<Project>(await Map(result.Data, token), result.PageCount);
        }

        protected Expression<Func<DB.Context.Project, bool>> GetFilter(ProjectFilter filter)
        {
            return s => s.UserId == filter.UserId && (filter.Name == null || s.Name.Contains(filter.Name)) && 
                        (filter.IsLeaf == null || s.IsLeaf == filter.IsLeaf) &&
                        (filter.LastUsedDateBegin == null || s.LastUsedDate >= filter.LastUsedDateBegin) &&
                        (filter.LastUsedDateEnd == null || s.LastUsedDate <= filter.LastUsedDateEnd) &&
                        (s.ParentId == filter.ParentId) &&
                        (filter.Path == null || s.Path.Contains(filter.Path));
        }

        protected async Task PrepareBeforeAdd(DB.Repository.IRepository<DB.Context.Project> repository,
            ProjectCreator creator, CancellationToken token)
        {           
            var settings = (await _settingsRepository.GetAsync(new DB.Context.Filter<DB.Context.UserSettings>()
            {
                Page = 0,
                Size = 10,
                Selector = s => s.UserId == creator.UserId
            }, token)).Data?.FirstOrDefault();

            if(settings!=null)
            {
                creator.Priority = settings.DefaultPriority;
            }

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

        protected async Task PrepareBeforeUpdate(DB.Repository.IRepository<DB.Context.Project> repository,
            ProjectUpdater entity, CancellationToken token)
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

        protected async Task PrepareBeforeDelete(DB.Repository.IRepository<DB.Context.Project> repository,
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

        protected DB.Context.Project UpdateFillFields(ProjectUpdater entity, DB.Context.Project entry)
        {
            entry.Path = entity.Path;
            entry.Name = entity.Name;
            entry.ParentId = entity.ParentId;
            entry.Period = entity.Period;
            entry.Priority = entity.Priority;
            return entry;
        }

        protected async Task<List<Project>> Map(IEnumerable<DB.Context.Project> entries, CancellationToken token)
        {
            var result = new List<Project>();
            foreach(var entry in entries)
            {
                result.Add(await Map(entry, token));
            }
            return result;
        }

        protected async Task<Project> Map(DB.Context.Project entry, CancellationToken token)
        {
            var entity = new Project()
            {
                AddTime = entry.AddTime,                
                IsLeaf = entry.IsLeaf,
                LastUsedDate = entry.LastUsedDate,
                Name = entry.Name,
                ParentId = entry.ParentId,
                Path = entry.Path,
                Period = entry.Period,
                Priority = entry.Priority,
                UserId = entry.UserId,
                VersionDate = entry.VersionDate,
                //todo
                //CanSelect = ,
                //CanSelectAll = ,                
                //Parent = ,               
            };
            return entity;
        }

        protected DB.Context.Project Map(ProjectCreator creator)
        {
            var entity = new DB.Context.Project()
            { 
                AddTime = creator.AddTime,
                IsDeleted = false,
                IsLeaf = true,
                LastUsedDate = DateTimeOffset.Now,
                Name = creator.Name,
                ParentId = creator.ParentId,
                Path = creator.Path,
                Period = creator.Period,
                Priority = creator.Priority,
                UserId = creator.UserId,
                VersionDate = DateTimeOffset.Now,
            };           
            return entity;
        }               

    }
}
