using AutoMapper;
using Planning.DB.Context;

namespace Planning
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, Contracts.Model.User>();

            CreateMap<Contracts.Model.UserCreator, User>()
                 .ForMember(s => s.Password, s => s.Ignore())
                 .ForMember(s => s.Formula, s => s.Ignore());

            CreateMap<UserHistory, Contracts.Model.UserHistory>();

            CreateMap<Contracts.Model.UserUpdater, User>()
                .ForMember(s => s.Password, s => s.Ignore())
                .ForMember(s => s.Formula, s => s.Ignore());

            CreateMap<Formula, Contracts.Model.Formula>();

            CreateMap<Contracts.Model.FormulaCreator, Formula>();

            CreateMap<FormulaHistory, Contracts.Model.FormulaHistory>();

            CreateMap<Contracts.Model.FormulaUpdater, Formula>();


            CreateMap<Project, Contracts.Model.Project>();

            CreateMap<Contracts.Model.ProjectCreator, Project>();

            CreateMap<ProjectHistory, Contracts.Model.ProjectHistory>();

            CreateMap<Contracts.Model.ProjectUpdater, Project>();


            CreateMap<Schedule, Contracts.Model.Schedule>();

            CreateMap<Contracts.Model.ScheduleCreator, Schedule>();

            CreateMap<ScheduleHistory, Contracts.Model.ScheduleHistory>();

            CreateMap<Contracts.Model.ScheduleUpdater, Schedule>();

        }
    }
}
