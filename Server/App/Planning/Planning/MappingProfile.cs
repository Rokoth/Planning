using AutoMapper;
using Planning.DB.Context;

namespace Planning
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, Contracts.Model.User.User>();

            CreateMap<Contracts.Model.User.UserCreator, User>()
                 .ForMember(s => s.Password, s => s.Ignore())
                 .ForMember(s => s.Formula, s => s.Ignore());

            CreateMap<UserHistory, Contracts.Model.User.UserHistory>();

            CreateMap<Contracts.Model.User.UserUpdater, User>()
                .ForMember(s => s.Password, s => s.Ignore())
                .ForMember(s => s.Formula, s => s.Ignore());

            CreateMap<Formula, Contracts.Model.User.Formula>();

            CreateMap<Contracts.Model.User.FormulaCreator, Formula>();

            CreateMap<FormulaHistory, Contracts.Model.User.FormulaHistory>();

            CreateMap<Contracts.Model.User.FormulaUpdater, Formula>();


            CreateMap<Project, Contracts.Model.Project.Project>();

            CreateMap<Contracts.Model.Project.ProjectCreator, Project>();

            CreateMap<ProjectHistory, Contracts.Model.Project.ProjectHistory>();

            CreateMap<Contracts.Model.Project.ProjectUpdater, Project>();


            CreateMap<Schedule, Contracts.Model.Schedule.Schedule>();

            CreateMap<Contracts.Model.Schedule.ScheduleCreator, Schedule>();

            CreateMap<ScheduleHistory, Contracts.Model.Schedule.ScheduleHistory>();

            CreateMap<Contracts.Model.Schedule.ScheduleUpdater, Schedule>();

        }
    }
}
