using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class UserHistoryDataService : DataGetService<DB.Context.UserHistory, Contract.Model.UserHistory,
        Contract.Model.UserHistoryFilter>
    {
        public UserHistoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Func<DB.Context.Filter<DB.Context.UserHistory>, CancellationToken, 
            Task<Contract.Model.PagedResult<DB.Context.UserHistory>>> GetListFunc(DB.Repository.IRepository<DB.Context.UserHistory> repo)
        {
            return repo.GetAsyncDeleted;
        }

        protected override Expression<Func<DB.Context.UserHistory, bool>> GetFilter(Contract.Model.UserHistoryFilter filter)
        {
            return s => (filter.Name == null || s.Name.Contains(filter.Name)) 
                && (filter.Id == null || s.Id == filter.Id);
        }
    }

    public class FormulaHistoryDataService : DataGetService<DB.Context.FormulaHistory, Contract.Model.FormulaHistory,
        Contract.Model.FormulaHistoryFilter>
    {
        public FormulaHistoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Func<DB.Context.Filter<DB.Context.FormulaHistory>, CancellationToken,
            Task<Contract.Model.PagedResult<DB.Context.FormulaHistory>>> GetListFunc(DB.Repository.IRepository<DB.Context.FormulaHistory> repo)
        {
            return repo.GetAsyncDeleted;
        }

        protected override Expression<Func<DB.Context.FormulaHistory, bool>> GetFilter(Contract.Model.FormulaHistoryFilter filter)
        {
            return s => (filter.Name == null || s.Name.Contains(filter.Name))
                && (filter.Id == null || s.Id == filter.Id);
        }
    }

    public class ProjectHistoryDataService : DataGetService<DB.Context.ProjectHistory, Contract.Model.ProjectHistory,
        Contract.Model.ProjectHistoryFilter>
    {
        public ProjectHistoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Func<DB.Context.Filter<DB.Context.ProjectHistory>, CancellationToken,
            Task<Contract.Model.PagedResult<DB.Context.ProjectHistory>>> GetListFunc(DB.Repository.IRepository<DB.Context.ProjectHistory> repo)
        {
            return repo.GetAsyncDeleted;
        }

        protected override Expression<Func<DB.Context.ProjectHistory, bool>> GetFilter(Contract.Model.ProjectHistoryFilter filter)
        {
            return s => (filter.Name == null || s.Name.Contains(filter.Name))
                && (filter.Id == null || s.Id == filter.Id);
        }
    }
}
