using Contracts.Model.Schedule;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Planning.Common;
using Planning.DB.Context;
using Planning.DB.Repository;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class ScheduleHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly bool isRunning = true;
        private readonly CancellationTokenSource _tokenSource;
        
        public ScheduleHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<ScheduleHostedService>>();
            _tokenSource = new CancellationTokenSource();
            
        }

        public async Task Run(CancellationToken _cancellationToken)
        {
            while (isRunning && !_cancellationToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var scopeProvider = scope.ServiceProvider;
                var userRepo = scopeProvider.GetRequiredService<IRepository<User>>();
                var userSettingsRepo = scopeProvider.GetRequiredService<IRepository<UserSettings>>();
                var scheduleRepo = scopeProvider.GetRequiredService<IRepository<DB.Context.Schedule>>();
                var selectService = scopeProvider.GetRequiredService<IProjectSelectService>();
                var notifyService = scopeProvider.GetRequiredService<INotifyDataService>();

                try
                {
                    var now = DateTimeOffset.Now;
                    var users = await userRepo.GetAsync(new Filter<User>()
                    {
                        Selector = s => true
                    }, _cancellationToken);
                    var userSettings = await userSettingsRepo.GetAsync(new Filter<UserSettings>()
                    {
                        Selector = s => true
                    }, _cancellationToken);

                    foreach (var user in users.Data)
                    {
                        var settings = userSettings.Data.FirstOrDefault(s => s.UserId == user.Id);

                        var currentSchedule = (await scheduleRepo.GetAsync(new Filter<DB.Context.Schedule>()
                        {
                            Selector = s => s.UserId == user.Id && !s.IsClosed && s.IsRunning
                        }, _cancellationToken)).Data.FirstOrDefault();

                        if (currentSchedule == null)
                        {
                            _logger.LogError($"Error in BuildScheduleHostedService: currentSchedule not found for user {user.Id} : {user.Name}");
                            return;
                        }

                        if (currentSchedule.EndDate <= now)
                        {
                            var next = await selectService.MoveToNextSchedule(user.Id);
                            await notifyService.AddNotify(user.Id, $"Переход на следующий элемент расписания: {next.Project}");
                        }
                        else if ((currentSchedule.EndDate - now).TotalMinutes < 5)
                        {
                            await notifyService.AddNotify(user.Id, $"Через {(currentSchedule.EndDate - now).TotalMinutes} минут будет переход на следующий элемент расписания");
                        }
                    }

                }
                catch (Exception ex)
                {                    
                    _logger.LogError($"Error in BuildScheduleHostedService: Run: {ex.Message} {ex.StackTrace}");
                }
                await Task.Delay(60000, _cancellationToken);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(() => Run(_tokenSource.Token), cancellationToken,
                TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            _tokenSource.Cancel();
        }
    }
}
