using System;

namespace Planning.Contracts.Model
{
    public class UserSettingsFilter : Filter<UserSettings>
    {
        public UserSettingsFilter(int size, int page, string sort, Guid userId) : base(size, page, sort)
        {
            UserId = userId;           
        }       
        public Guid UserId { get; set; }
    }
}
