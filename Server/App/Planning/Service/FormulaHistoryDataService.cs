using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class FormulaHistoryDataService : DataGetService<DB.Context.FormulaHistory, Contracts.Model.FormulaHistory,
        Contracts.Model.FormulaHistoryFilter>
    {
        public FormulaHistoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Func<DB.Context.Filter<DB.Context.FormulaHistory>, CancellationToken,
            Task<Contracts.Model.PagedResult<DB.Context.FormulaHistory>>> GetListFunc(DB.Repository.IRepository<DB.Context.FormulaHistory> repo)
        {
            return repo.GetAsyncDeleted;
        }

        protected override Expression<Func<DB.Context.FormulaHistory, bool>> GetFilter(Contracts.Model.FormulaHistoryFilter filter)
        {
            return s => (filter.Name == null || s.Name.Contains(filter.Name))
                && (filter.Id == null || s.Id == filter.Id);
        }
    }
}
