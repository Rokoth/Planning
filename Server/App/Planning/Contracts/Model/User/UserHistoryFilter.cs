using Contracts.Model.Common;
using Contracts.Model.Direction;
using System;

namespace Contracts.Model.User
{
    public class UserHistoryFilter : Filter<UserHistory>
    {
        public UserHistoryFilter(int size, int page, string sort, string name, Guid? id) : base(size, page, sort)
        {
            Name = name;
            Id = id;
        }
        public string Name { get; }
        public Guid? Id { get; }
    }

    public class DirectionHistoryFilter : Filter<DirectionHistory>
    {
        public DirectionHistoryFilter(int size, int page, string sort, string name, Guid? id) : base(size, page, sort)
        {
            Name = name;
            Id = id;
        }
        public string Name { get; }
        public Guid? Id { get; }
    }

    public class DirectionCategoryHistoryFilter : Filter<DirectionCategoryHistory>
    {
        public DirectionCategoryHistoryFilter(int size, int page, string sort, string name, Guid? id) : base(size, page, sort)
        {
            Name = name;
            Id = id;
        }
        public string Name { get; }
        public Guid? Id { get; }
    }

    public class DirectionProjectHistoryFilter : Filter<DirectionProjectHistory>
    {
        public DirectionProjectHistoryFilter(int size, int page, string sort, string name, Guid? id) : base(size, page, sort)
        {
            Name = name;
            Id = id;
        }
        public string Name { get; }
        public Guid? Id { get; }
    }


}
