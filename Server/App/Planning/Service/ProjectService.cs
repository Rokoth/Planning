
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class ProjectService
    {
        private readonly DB.Repository.IRepository<Planning.DB.Context.Project> repositoryProject;
       
        private readonly ILogger<ProjectService> logger;
        private readonly IMapper mapper;

        public ProjectService(IServiceProvider serviceProvider)
        {
            repositoryProject = serviceProvider.GetRequiredService<DB.Repository.IRepository<Planning.DB.Context.Project>>();
            
            mapper = serviceProvider.GetRequiredService<IMapper>();
        }

        public async Task<IEnumerable<Planning.Contract.Model.Project>> GetProjects(Contract.Model.ProjectFilter filter)
        {
            var result = await repositoryProject.GetList(s =>
                (filter.IsLeaf == null || s.IsProject == filter.IsLeaf)
                    && (filter.LastUsedDateBegin == null || s.LastUsedDate >= filter.LastUsedDateBegin)
                    && (filter.LastUsedDateEnd == null || s.LastUsedDate <= filter.LastUsedDateEnd)
                    && (filter.Name == null || s.Name == filter.Name)
                    && (filter.ParentId == null || s.ParentId == filter.ParentId)
                );


            return result.Select(s => mapper.Map<Planning.Contract.Model.Project>(s));
        }
    }


}
