using Contracts.Model.Common;
using Contracts.Model.Direction;
using Contracts.Model.Formula;
using Contracts.Model.Project;
using Contracts.Model.Schedule;
using Contracts.Model.User;
using Microsoft.Extensions.DependencyInjection;

namespace Planning.Service
{
    public static class DataServiceExtension
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            services.AddScoped<IScheduleDataService, ScheduleDataService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<INotifyDataService, NotifyDataService>();

            services.AddDataService<UserDataService, DB.Context.User, User,
                UserFilter, UserCreator, UserUpdater>();
            services.AddDataService<FormulaDataService, DB.Context.Formula, Formula,
                FormulaFilter, FormulaCreator, FormulaUpdater>();
            services.AddDataService<ProjectDataService, DB.Context.Project, Project,
                ProjectFilter, ProjectCreator, ProjectUpdater>();
            

            services.AddDataService<DirectionCategoryDataService, DB.Context.DirectionCategory, DirectionCategory,
               DirectionCategoryFilter, DirectionCategoryCreator, DirectionCategoryUpdater>();
            services.AddDataService<DirectionDataService, DB.Context.Direction, Direction,
               DirectionFilter, DirectionCreator, DirectionUpdater>();
            services.AddDataService<DirectionProjectDataService, DB.Context.DirectionProject, DirectionProject,
               DirectionProjectFilter, DirectionProjectCreator, DirectionProjectUpdater>();

            services.AddScoped<IGetDataService<UserHistory, UserHistoryFilter>, UserHistoryDataService>();
            services.AddScoped<IGetDataService<FormulaHistory, FormulaHistoryFilter>, FormulaHistoryDataService>();
            services.AddScoped<IGetDataService<ProjectHistory, ProjectHistoryFilter>, ProjectHistoryDataService>();
            services.AddScoped<IGetDataService<ScheduleHistory, ScheduleHistoryFilter>, ScheduleHistoryDataService>();

            services.AddScoped<IGetDataService<DirectionCategoryHistory, DirectionCategoryHistoryFilter>, DirectionCategoryHistoryDataService>();
            services.AddScoped<IGetDataService<DirectionHistory, DirectionHistoryFilter>, DirectionHistoryDataService>();
            services.AddScoped<IGetDataService<DirectionProjectHistory, DirectionProjectHistoryFilter>, DirectionProjectHistoryDataService>();
            

            return services;
        }
       
    }
}
