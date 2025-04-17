using Contracts.Model.Common;
using Contracts.Model.User;
using System;

namespace Contracts.Model.Formula
{
    public class FormulaHistoryFilter : Filter<FormulaHistory>
    {
        public FormulaHistoryFilter(Guid id, int size, int page, string sort, string name) : base(size, page, sort)
        {
            Name = name;
            Id = id;
        }
        public string Name { get; }
        public Guid Id { get; }
    }
}
