
using AutoMapper;
using Contract.Model;
using DB.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class ProjectService
    {
        private readonly IRepository<DB.Context.Project> repositoryProject;
        private readonly IRepository<DB.Context.ProjectPeriod> repositoryProjectPeriod;
        private readonly ILogger<ProjectService> logger;
        private readonly IMapper mapper;

        public ProjectService(IServiceProvider serviceProvider)
        {
            repositoryProject = serviceProvider.GetRequiredService<IRepository<DB.Context.Project>>();
            repositoryProjectPeriod = serviceProvider.GetRequiredService<IRepository<DB.Context.ProjectPeriod>>();
            mapper = serviceProvider.GetRequiredService<IMapper>();
        }

        public async Task<IEnumerable<Project>> GetProjects(ProjectFilter filter)
        {
            var result = await repositoryProject.GetList(s =>
                (filter.IsProject == null || s.IsProject == filter.IsProject)
                    && (filter.LastUsedDateBegin == null || s.LastUsedDate >= filter.LastUsedDateBegin)
                    && (filter.LastUsedDateEnd == null || s.LastUsedDate <= filter.LastUsedDateEnd)
                    && (filter.Name == null || s.Name == filter.Name)
                    && (filter.ParentId == null || s.ParentId == filter.ParentId)
                );


            return result.Select(s => mapper.Map<Project>(s));
        }
    }


}
