using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IIntegrationService
    {
        Task<bool> BuhgalteryAddReserve(string taskData, CancellationToken token);
    }
}