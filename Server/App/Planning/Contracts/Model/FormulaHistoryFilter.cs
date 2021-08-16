using System;

namespace Planning.Contract.Model
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
