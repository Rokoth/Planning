using Planning.DB.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class NotifyDataService : INotifyDataService
    {
        IRepository<DB.Context.Notify> _notifyRepo;

        public NotifyDataService(IRepository<DB.Context.Notify> notifyRepo)
        {
            _notifyRepo = notifyRepo;
        }

        public async Task<IEnumerable<Contracts.Model.Project.Notify>> GetNotifiesAsync(Guid userId)
        {
            var data = await _notifyRepo.GetAsync(new DB.Context.Filter<DB.Context.Notify>()
            {
                Selector = s => !s.IsDeleted && s.UserId == userId && !s.IsSend
            }, new CancellationToken());

            return data.Data.Select(s => new Contracts.Model.Project.Notify()
            {
                Id = s.Id,
                UserId = s.UserId,
                Text = s.Text,
                IsSend = s.IsSend
            });
        }

        public async Task AddNotify(Guid userId, string text)
        {
            await _notifyRepo.AddAsync(new DB.Context.Notify()
            {
                IsDeleted = false,
                IsSend = false,
                Text = text,
                UserId = userId
            }, true, new CancellationToken());
        }

        public async Task SetNotifySend(Guid id)
        {
            var token = new CancellationToken();
            var notify = await _notifyRepo.GetAsync(id, token);
            notify.IsSend = true;
            if (notify != null)
            {
                await _notifyRepo.UpdateAsync(notify, true, new CancellationToken());
            }
        }
    }
}
