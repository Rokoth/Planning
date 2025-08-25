using Contracts.Model.Common;
using Contracts.Model.User;
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
    public class UserDataService : DataService<DB.Context.User, User>, IUserDataService
    {
        public UserDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task<PagedResult<User>> GetAsync(UserFilter filter, CancellationToken token)
        {
            return await ExecuteListAsync(async (repo) =>
            {
                string sort = filter.Sort;
                if (string.IsNullOrEmpty(sort))
                {
                    sort = DefaultSort;
                }

                var data = await repo.GetAsync(new DB.Context.Filter<DB.Context.User>
                {
                    Size = filter.Size,
                    Page = filter.Page,
                    Sort = sort,
                    Selector = GetFilter(filter)
                }, token);
                var result = new List<User>();
                foreach (var item in data.Data)
                {
                    result.Add(await Map(item, token));
                }
                return new PagedResult<User>(result, data.PageCount);
            });
        }

        public async Task<User> GetAsync(Guid id, CancellationToken token)
        {
            return await ExecuteAsync(async (repo) =>
            {
                var result = await repo.GetAsync(id, token);
                var prepare = await Map(result, token);
                return prepare;
            });
        }

        /// <summary>
        /// add item method
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<User> AddAsync(UserCreator creator, CancellationToken token)
        {
            return await ExecuteAsync(async (repo) =>
            {
                var entity = MapToEntityAdd(creator);
                var result = await repo.AddAsync(entity, false, token);
                await ActionAfterAdd(creator, result, token);
                await repo.SaveChangesAsync();
                return await Map(result, token);
            });
        }

        public async Task<User> UpdateAsync(UserUpdater entity, CancellationToken token)
        {
            return await ExecuteAsync(async (repo) =>
            {
                var entry = await repo.GetAsync(entity.Id, token);
                entry = UpdateFillFields(entity, entry);
                var result = await repo.UpdateAsync(entry, false, token);
                await ActionAfterUpdate(entity, result, token);
                await repo.SaveChangesAsync();
                return await Map(result, token);
            });
        }

        public async Task<User> DeleteAsync(Guid id, CancellationToken token)
        {
            return await ExecuteAsync(async (repo) =>
            {
                var entity = await repo.GetAsync(id, token) ??
                    throw new DataServiceException($"Entity with id = {id} not found in DB");
                entity = await repo.DeleteAsync(entity, false, token);
                await ActionAfterDelete(entity, token);
                await repo.SaveChangesAsync();
                return await Map(entity, token);
            });
        }

        /// <summary>
        /// function for enrichment data item
        /// </summary>
        private async Task<User> Map(DB.Context.User entity, CancellationToken token)
        {
            User result = new()
            {
                Description = entity.Description,
                FormulaId = entity.FormulaId,
                Id = entity.Id,
                Login = entity.Login,
                Name = entity.Name,
                VersionDate = entity.VersionDate
            };

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
            result.Formula = formula.Name;
            result.DefaultProjectTimespan = userSettings.DefaultProjectTimespan;
            result.LeafOnly = userSettings.LeafOnly;
            result.ScheduleCount = userSettings.ScheduleCount;
            result.ScheduleMode = userSettings.ScheduleMode;
            result.ScheduleShift = userSettings.ScheduleShift;
            result.ScheduleTimeSpan = userSettings.ScheduleTimeSpan;
            return result;
        }

        private static Expression<Func<DB.Context.User, bool>> GetFilter(UserFilter filter)
        {
            return s => filter.Name == null || s.Name.Contains(filter.Name);
        }

        private async Task ActionAfterAdd(UserCreator creator, DB.Context.User entity, CancellationToken token)
        {
            var userSettingsRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.UserSettings>>();
            await userSettingsRepo.AddAsync(new DB.Context.UserSettings()
            {
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

        private async Task ActionAfterUpdate(UserUpdater updater, DB.Context.User entity, CancellationToken token)
        {
            var userSettingsRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.UserSettings>>();
            var userSettings = (await userSettingsRepo.GetAsync(new DB.Context.Filter<DB.Context.UserSettings>()
            {
                Selector = s => s.UserId == entity.Id
            }, token)).Data.Single();

            userSettings.DefaultProjectTimespan = updater.DefaultProjectTimespan;
            userSettings.LeafOnly = updater.LeafOnly;
            userSettings.ScheduleCount = updater.ScheduleCount;
            userSettings.ScheduleMode = updater.ScheduleMode;
            userSettings.ScheduleShift = updater.ScheduleShift;
            userSettings.ScheduleTimeSpan = updater.ScheduleTimeSpan;

            await userSettingsRepo.UpdateAsync(userSettings, false, token);
        }

        private async Task ActionAfterDelete(DB.Context.User entity, CancellationToken token)
        {
            var userSettingsRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.UserSettings>>();
            var userSettings = (await userSettingsRepo.GetAsync(new DB.Context.Filter<DB.Context.UserSettings>()
            {
                Selector = s => s.UserId == entity.Id
            }, token)).Data.Single();

            await userSettingsRepo.DeleteAsync(userSettings, false, token);
        }

        private static DB.Context.User MapToEntityAdd(UserCreator creator)
        {
            var entity = new DB.Context.User
            {
                Description = creator.Description,
                Login = creator.Login,
                Name = creator.Name,
                Password = SHA512.HashData(Encoding.UTF8.GetBytes(creator.Password)),
                FormulaId = creator.FormulaId,
                VersionDate = DateTimeOffset.Now,
                Id = Guid.NewGuid()
            };
            return entity;
        }

        private static DB.Context.User UpdateFillFields(UserUpdater entity, DB.Context.User entry)
        {
            entry.Description = entity.Description;
            entry.Login = entity.Login;
            entry.Name = entity.Name;
            if (entity.PasswordChanged)
            {
                entry.Password = SHA512.HashData(Encoding.UTF8.GetBytes(entity.Password));
            }
            return entry;
        }

        private static string DefaultSort => "Name";
    }
}
