using Planning.Contract.Model;
using System;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IProjectSelectService
    {
        Task<Project> AddProjectToSchedule(Guid userId, bool leafOnly, DateTimeOffset beginDate, Guid? projectId = null);
    }
}