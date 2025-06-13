using Contracts.Model.Project;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface INotifyDataService
    {
        Task AddNotify(Guid userId, string text);
        Task<IEnumerable<Notify>> GetNotifiesAsync(Guid userId);
        Task SetNotifySend(Guid id);
    }
}