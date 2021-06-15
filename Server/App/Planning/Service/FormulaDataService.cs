using System;
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
            return s => filter.Name == null || s.Name.Contains(filter.Name);
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

    public class ProjectDataService : DataService<DB.Context.Project, Contract.Model.Project,
       Contract.Model.ProjectFilter, Contract.Model.ProjectCreator, Contract.Model.ProjectUpdater>
    {
        public ProjectDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Expression<Func<DB.Context.Project, bool>> GetFilter(Contract.Model.ProjectFilter filter)
        {
            return s => (filter.Name == null || s.Name.Contains(filter.Name)) && 
                        (filter.IsLeaf == null || s.IsLeaf == filter.IsLeaf) &&
                        (filter.LastUsedDateBegin == null || s.LastUsedDate >= filter.LastUsedDateBegin) &&
                        (filter.LastUsedDateEnd == null || s.LastUsedDate <= filter.LastUsedDateEnd) &&
                        (filter.ParentId == null || s.ParentId == filter.ParentId);
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
            var parent = await repository.GetAsync(new DB.Context.Filter<DB.Context.Project>()
            {
                Page = 0,
                Size = 10,
                Selector = s => s.Id == entity.ParentId
            }, token);
            foreach (var item in parent.Data)
            {
                var childs = await repository.GetAsync(new DB.Context.Filter<DB.Context.Project>()
                {
                    Page = 0,
                    Size = 10,
                    Selector = s => s.ParentId == item.Id
                }, token);
                if (!childs.Data.Any())
                {
                    item.IsLeaf = true;
                    await repository.UpdateAsync(item, false, token);
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

        protected override string DefaultSort => "Name";

    }
}
