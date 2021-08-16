using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Planning.DB.Context;
using Planning.DB.Repository;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class BuildScheduleHostedService : IHostedService
    {
        private IServiceProvider _serviceProvider;
        private ILogger _logger;
        private bool isRunning = true;
        private CancellationTokenSource _tokenSource;

        public BuildScheduleHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<BuildScheduleHostedService>>();
            _tokenSource = new CancellationTokenSource();
        }

        public async Task Run(CancellationToken _cancellationToken)
        {
            while (isRunning && !_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var now = DateTimeOffset.Now;
                        var scopeProvider = scope.ServiceProvider;
                        var userRepo = scopeProvider.GetRequiredService<IRepository<User>>();
                        var userSettingsRepo = scopeProvider.GetRequiredService<IRepository<UserSettings>>();
                        var scheduleRepo = scopeProvider.GetRequiredService<IRepository<Schedule>>();
                        var selectService = scopeProvider.GetRequiredService<IProjectSelectService>();

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
                            await selectService.ShiftSchedule(user.Id, settings, now.AddMinutes(settings.ScheduleShift));

                            var currentSchedules = await scheduleRepo.GetAsync(new Filter<Schedule>()
                            {
                                Selector = s => s.UserId == user.Id && s.EndDate >= now
                            }, _cancellationToken);

                            switch (settings.ScheduleMode)
                            {
                                case Contract.Model.ScheduleMode.ByCount:
                                    if (settings.ScheduleCount.Value > currentSchedules.Data.Count())
                                    {
                                        for (int i = 0; i < settings.ScheduleCount.Value - currentSchedules.Data.Count(); i++)
                                        {
                                            await selectService.AddProjectToSchedule(user.Id, settings);
                                        }
                                    }

                                    break;
                                case Contract.Model.ScheduleMode.ByTimeSpan:
                                    var lastSchedule = currentSchedules.Data.OrderByDescending(s => s.EndDate).FirstOrDefault().EndDate;
                                    var lastTime = now.AddHours(settings.ScheduleTimeSpan.Value);
                                    while (lastSchedule < lastTime)
                                    {
                                        lastSchedule = (await selectService.AddProjectToSchedule(user.Id, settings)).EndDate;
                                    }
                                    break;
                                default: break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in BuildScheduleHostedService: Run: {ex.Message} {ex.StackTrace}");
                }
                await Task.Delay(60000);
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
