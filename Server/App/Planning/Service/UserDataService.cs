using System;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

namespace Planning.Service
{
    public class UserDataService : DataService<DB.Context.User, Contract.Model.User,
        Contract.Model.UserFilter, Contract.Model.UserCreator, Contract.Model.UserUpdater>
    {
        public UserDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Expression<Func<DB.Context.User, bool>> GetFilter(Contract.Model.UserFilter filter)
        {
            return s => filter.Name == null || s.Name.Contains(filter.Name);
        }

        protected override DB.Context.User MapToEntityAdd(Contract.Model.UserCreator creator)
        {
            var entity = base.MapToEntityAdd(creator);
            entity.Password = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(creator.Password));
            return entity;
        }

        protected override DB.Context.User UpdateFillFields(Contract.Model.UserUpdater entity, DB.Context.User entry)
        {
            entry.Description = entity.Description;
            entry.Login = entity.Login;
            entry.Name = entity.Name;
            if (entity.PasswordChanged)
            {
                entry.Password = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(entity.Password));
            }
            return entry;
        }

        protected override string DefaultSort => "Name";
        
    }
}
