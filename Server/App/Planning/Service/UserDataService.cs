using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class UserDataService : DataService<DB.Context.User, Contract.Model.User,
        Contract.Model.UserFilter, Contract.Model.UserCreator, Contract.Model.UserUpdater>
    {
        public UserDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        /// <summary>
        /// function for enrichment data item
        /// </summary>
        protected override async Task<Contract.Model.User> Enrich(Contract.Model.User entity, CancellationToken token)
        {
            var userSettingsRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.UserSettings>>();
            var formulaRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Formula>>();
            var userSettings = (await userSettingsRepo.GetAsync(new DB.Context.Filter<DB.Context.UserSettings>()
            {
                Selector = s => s.UserId == entity.Id
            }, token)).Data.Single();
            var formula = (await formulaRepo.GetAsync(new DB.Context.Filter<DB.Context.Formula>()
            {
                Selector = s => s.Id == entity.FormulaId
            }, token)).Data.Single();
            entity.Formula = formula.Name;
            entity.DefaultProjectTimespan = userSettings.DefaultProjectTimespan;
            entity.LeafOnly = userSettings.LeafOnly;
            entity.ScheduleCount = userSettings.ScheduleCount;
            entity.ScheduleMode = userSettings.ScheduleMode;
            entity.ScheduleShift = userSettings.ScheduleShift;
            entity.ScheduleTimeSpan = userSettings.ScheduleTimeSpan;
            return entity;
        }

        /// <summary>
        /// function for enrichment data item
        /// </summary>
        protected override async Task<IEnumerable<Contract.Model.User>> Enrich(IEnumerable<Contract.Model.User> entities, CancellationToken token)
        {
            var result = new List<Contract.Model.User>();
            var userSettingsRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.UserSettings>>();
            var formulaRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Formula>>();
            var userIds = entities.Select(s => s.Id).ToList();
            var formulaIds = entities.Select(s => s.FormulaId).Distinct().ToList();
            var userSettingss = (await userSettingsRepo.GetAsync(new DB.Context.Filter<DB.Context.UserSettings>()
            {
                Selector = s => userIds.Contains(s.UserId)
            }, token)).Data.ToList();
            var formulas = (await formulaRepo.GetAsync(new DB.Context.Filter<DB.Context.Formula>()
            {
                Selector = s => formulaIds.Contains(s.Id)
            }, token)).Data.ToList();

            foreach (var entity in entities)
            {
                var formula = formulas.Where(s=>s.Id == entity.FormulaId).Single();
                var userSettings = userSettingss.Where(s => s.UserId == entity.Id).Single();
                entity.Formula = formula.Name;
                entity.DefaultProjectTimespan = userSettings.DefaultProjectTimespan;
                entity.LeafOnly = userSettings.LeafOnly;
                entity.ScheduleCount = userSettings.ScheduleCount;
                entity.ScheduleMode = userSettings.ScheduleMode;
                entity.ScheduleShift = userSettings.ScheduleShift;
                entity.ScheduleTimeSpan = userSettings.ScheduleTimeSpan;
                result.Add(entity);
            }

            return result;
        }

        protected override Expression<Func<DB.Context.User, bool>> GetFilter(Contract.Model.UserFilter filter)
        {
            return s => filter.Name == null || s.Name.Contains(filter.Name);
        }

        protected override async Task ActionAfterAdd(DB.Repository.IRepository<DB.Context.User> repository,
            Contract.Model.UserCreator creator, DB.Context.User entity, CancellationToken token)
        {
            var userSettingsRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.UserSettings>>();
            await userSettingsRepo.AddAsync(new DB.Context.UserSettings() { 
               DefaultProjectTimespan = creator.DefaultProjectTimespan,
               Id = Guid.NewGuid(),
               IsDeleted = false,
               LeafOnly = creator.LeafOnly,
               ScheduleCount = creator.ScheduleCount,
               ScheduleMode = creator.ScheduleMode,
               ScheduleShift = creator.ScheduleShift,
               ScheduleTimeSpan = creator.ScheduleTimeSpan,
               UserId = entity.Id,
               VersionDate = DateTimeOffset.Now
            }, false, token);
        }

        protected override async Task ActionAfterUpdate(DB.Repository.IRepository<DB.Context.User> repository,
            Contract.Model.UserUpdater updater, DB.Context.User entity, CancellationToken token)
        {
            var userSettingsRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.UserSettings>>();
            var userSettings = (await userSettingsRepo.GetAsync(new DB.Context.Filter<DB.Context.UserSettings>()
            { 
               Selector = s=>s.UserId == entity.Id
            }, token)).Data.Single();

            userSettings.DefaultProjectTimespan = updater.DefaultProjectTimespan;
            userSettings.LeafOnly = updater.LeafOnly;
            userSettings.ScheduleCount = updater.ScheduleCount;
            userSettings.ScheduleMode = updater.ScheduleMode;
            userSettings.ScheduleShift = updater.ScheduleShift;
            userSettings.ScheduleTimeSpan = updater.ScheduleTimeSpan;

            await userSettingsRepo.UpdateAsync(userSettings, false, token);
        }

        protected override async Task ActionAfterDelete(DB.Repository.IRepository<DB.Context.User> repository,
            DB.Context.User entity, CancellationToken token)
        {
            var userSettingsRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.UserSettings>>();
            var userSettings = (await userSettingsRepo.GetAsync(new DB.Context.Filter<DB.Context.UserSettings>()
            {
                Selector = s => s.UserId == entity.Id
            }, token)).Data.Single();                       

            await userSettingsRepo.DeleteAsync(userSettings, false, token);
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
