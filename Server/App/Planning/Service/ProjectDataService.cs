using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class ProjectDataService : DataService<DB.Context.Project, Contract.Model.Project,
       Contract.Model.ProjectFilter, Contract.Model.ProjectCreator, Contract.Model.ProjectUpdater>
    {
        public ProjectDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Expression<Func<DB.Context.Project, bool>> GetFilter(Contract.Model.ProjectFilter filter)
        {
            return s => s.UserId == filter.UserId && (filter.Name == null || s.Name.Contains(filter.Name)) && 
                        (filter.IsLeaf == null || s.IsLeaf == filter.IsLeaf) &&
                        (filter.LastUsedDateBegin == null || s.LastUsedDate >= filter.LastUsedDateBegin) &&
                        (filter.LastUsedDateEnd == null || s.LastUsedDate <= filter.LastUsedDateEnd) &&
                        (s.ParentId == filter.ParentId) &&
                        (filter.Path == null || s.Path.Contains(filter.Path));
        }

        protected override async Task PrepareBeforeAdd(DB.Repository.IRepository<DB.Context.Project> repository,
            Contract.Model.ProjectCreator creator, CancellationToken token)
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
            Contract.Model.ProjectUpdater entity, CancellationToken token)
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

        protected override DB.Context.Project UpdateFillFields(Contract.Model.ProjectUpdater entity, DB.Context.Project entry)
        {
            entry.Path = entity.Path;
            entry.Name = entity.Name;
            entry.ParentId = entity.ParentId;
            entry.Period = entity.Period;
            entry.Priority = entity.Priority;
            return entry;
        }

        protected override DB.Context.Project MapToEntityAdd(Contract.Model.ProjectCreator creator)
        {
            var entity = base.MapToEntityAdd(creator);
            entity.LastUsedDate = DateTimeOffset.Now;
            return entity;
        }

        protected override string DefaultSort => "Name";

    }
}
