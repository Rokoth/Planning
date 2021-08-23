using System;

namespace Planning.Contract.Model
{
    public class FormulaFilter : Filter<Formula>
    {
        public FormulaFilter(int size, int page, string sort, string name, bool? isDefault) : base(size, page, sort)
        {
            Name = name;
            IsDefault = isDefault;
        }
        public string Name { get; }
        public bool? IsDefault { get; }
    }

    public class UserSettingsFilter : Filter<UserSettings>
    {
        public UserSettingsFilter(int size, int page, string sort, Guid userId) : base(size, page, sort)
        {
            UserId = userId;           
        }       
        public Guid UserId { get; set; }
    }
}
