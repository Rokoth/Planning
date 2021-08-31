using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Planning.Common;
using Planning.DB.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{

    public class ProjectSelectService : IProjectSelectService
    {
        private IServiceProvider _serviceProvider;
        private ILogger _logger;
        private IMapper _mapper;
        private readonly IErrorNotifyService errorNotifyService;

        public ProjectSelectService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<ProjectSelectService>>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            errorNotifyService = _serviceProvider.GetRequiredService<IErrorNotifyService>();
        }

        public async Task MoveNextSchedule(Guid userId, UserSettings settings)
        {
            try
            {
                var now = DateTimeOffset.Now;
                await ShiftSchedule(userId, settings, now);

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(30000);
                var _scheduleRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Schedule>>();
                var _projectRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Project>>();
                var userSettingsRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<UserSettings>>();

                var currentSchedules = (await _scheduleRepo.GetAsync(new DB.Context.Filter<DB.Context.Schedule>()
                {
                    Selector = s => s.UserId == userId && (s.IsRunning || s.EndDate > now)
                }, cancellationTokenSource.Token)).Data;

                var userSettings = (await userSettingsRepo.GetAsync(new Filter<UserSettings>()
                {
                    Selector = s => s.UserId == userId
                }, cancellationTokenSource.Token)).Data.FirstOrDefault();

                var runningSchedule = currentSchedules.FirstOrDefault(s => s.IsRunning);
                if (runningSchedule != null)
                {
                    runningSchedule.IsRunning = false;                    
                    await _scheduleRepo.UpdateAsync(runningSchedule, false, cancellationTokenSource.Token);

                    var currentProject = await _projectRepo.GetAsync(runningSchedule.ProjectId, cancellationTokenSource.Token);
                    currentProject.LastUsedDate = now;
                    if (currentProject.Period.HasValue)
                        currentProject.AddTime += (currentProject.Period.Value - (int)((runningSchedule.EndDate - runningSchedule.BeginDate).TotalMinutes));
                    await _projectRepo.UpdateAsync(currentProject, false, cancellationTokenSource.Token);
                }

                var nextSchedule = currentSchedules.Where(s => s.EndDate > now).OrderBy(s => s.BeginDate).FirstOrDefault();
                if (nextSchedule == null) nextSchedule = await AddProjectToSchedule(userId, userSettings);
                nextSchedule.IsRunning = true;
                await _scheduleRepo.UpdateAsync(nextSchedule, false, cancellationTokenSource.Token);

                await _projectRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await errorNotifyService.Send($"Error in ProjectSelectService:: MoveNextSchedule: {ex.Message} {ex.StackTrace}");
                _logger.LogError($"Error in ProjectSelectService:: MoveNextSchedule: {ex.Message} {ex.StackTrace}");
                throw;
            }
        }

        public async Task ShiftSchedule(Guid userId, UserSettings settings, DateTimeOffset now)
        {
            try
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(30000);
                var _scheduleRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Schedule>>();
                var _projectRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Project>>();
                var userSettingsRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<UserSettings>>();
                var currentSchedules = (await _scheduleRepo.GetAsync(new DB.Context.Filter<DB.Context.Schedule>()
                {
                    Selector = s => s.UserId == userId && (s.IsRunning || s.EndDate > now)
                }, cancellationTokenSource.Token)).Data.OrderBy(s => s.BeginDate);
                                
                var beginDate = now;
                
                foreach (var schedule in currentSchedules)
                {
                    var project = await _projectRepo.GetAsync(schedule.ProjectId, cancellationTokenSource.Token);
                    if (schedule.IsRunning)
                    {
                        schedule.EndDate = now;                        
                    }
                    else
                    {
                        schedule.BeginDate = beginDate;
                        if (project.Period.HasValue)
                        {
                            schedule.EndDate = schedule.BeginDate.AddMinutes(Math.Max(project.Period.Value + project.AddTime, 0));
                        }
                        else
                        {
                            schedule.EndDate = schedule.BeginDate.AddMinutes(settings.DefaultProjectTimespan);
                        }
                        beginDate = schedule.EndDate;
                    }
                    
                    await _scheduleRepo.UpdateAsync(schedule, false, cancellationTokenSource.Token);
                }
                await _projectRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await errorNotifyService.Send($"Error in ProjectSelectService:: ShiftSchedule: {ex.Message} {ex.StackTrace}");
                _logger.LogError($"Error in ProjectSelectService::ShiftSchedule: {ex.Message} {ex.StackTrace}");
                throw;
            }
        }

        public async Task<Schedule> AddProjectToSchedule(Guid userId, UserSettings settings, Guid? projectId = null, DateTimeOffset? beginDate = null, bool setBeginDate = false)
        {
            try
            {
                DB.Context.Project project = null;
                var now = DateTimeOffset.Now;
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(30000);
                var _calculator = _serviceProvider.GetRequiredService<ICalculator>();
                var _projectRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Project>>();
                var _scheduleRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Schedule>>();
                var _userRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.User>>();
                var _formulaRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Formula>>();

                var currentSchedules = (await _scheduleRepo.GetAsync(new DB.Context.Filter<DB.Context.Schedule>()
                {
                    Selector = s => s.UserId == userId && s.EndDate > now
                }, cancellationTokenSource.Token)).Data.OrderBy(s => s.BeginDate);

                var allProjects = await _projectRepo.GetAsync(
                        new DB.Context.Filter<DB.Context.Project>()
                        {
                            Selector = s => s.UserId == userId
                                && (!settings.LeafOnly || s.IsLeaf)
                        },
                        cancellationTokenSource.Token);

                var lastSchedule = currentSchedules.Last();
                                
                if (projectId != null)
                {
                    project = await _projectRepo.GetAsync(projectId.Value, cancellationTokenSource.Token);
                }
                else
                {
                    var user = await _userRepo.GetAsync(userId, cancellationTokenSource.Token);
                    var formula = await _formulaRepo.GetAsync(user.FormulaId, cancellationTokenSource.Token);
                    List<CalcRequestItem> items = new List<CalcRequestItem>();
                    

                    foreach (var item in allProjects.Data)
                    {
                        var fields = JObject.FromObject(item);
                        if (item.LastUsedDate.HasValue)
                        {
                            var lostHours = ((lastSchedule.EndDate - item.LastUsedDate.Value).TotalHours);
                            fields.Add("LostHours", lostHours);
                        }
                        var request = new CalcRequestItem()
                        {
                            Id = item.Id,
                            Fields = fields.ToString()
                        };
                        items.Add(request);
                    }

                    var result = _calculator.Calculate(new CalcRequest()
                    {
                        Count = 1,
                        Formula = formula.Text,
                        Items = items
                    }).FirstOrDefault();

                    if (result != null)
                    {
                        project = allProjects.Data.FirstOrDefault(s => s.Id == result.Id);
                    }
                }
                if (project != null)
                {
                    Schedule schedule = new DB.Context.Schedule()
                    {
                        BeginDate = lastSchedule.EndDate,
                        Id = Guid.NewGuid(),
                        IsRunning = false,
                        IsDeleted = false,
                        Order = lastSchedule?.Order + 1 ?? 1,
                        ProjectId = project.Id,
                        UserId = userId,
                        VersionDate = DateTimeOffset.Now
                    };
                    
                    if (setBeginDate)
                    {
                        var bDate = beginDate ?? now;
                        schedule.BeginDate = bDate;
                        SetEndDate(settings, project, schedule);
                        var toUpdate = currentSchedules.Where(s => s.EndDate > bDate).OrderBy(s => s.BeginDate).ToList();
                        if (toUpdate.Any())
                        {
                            var currShed = toUpdate[0];
                            currShed.EndDate = bDate;
                            schedule.Order = currShed.Order + 1;
                            var currproj = allProjects.Data.FirstOrDefault(s => s.Id == currShed.ProjectId);
                            if(currproj.Period.HasValue) 
                                currproj.AddTime += currproj.Period.Value - ((int)(currShed.EndDate - currShed.BeginDate).TotalMinutes);
                            await _scheduleRepo.UpdateAsync(currShed, false, cancellationTokenSource.Token);
                            await _projectRepo.UpdateAsync(currproj, false, cancellationTokenSource.Token);
                            var nextBDate = schedule.EndDate;
                            for (int i = 1; i < toUpdate.Count; i++)
                            {
                                var shed = toUpdate[i];
                                shed.BeginDate = nextBDate;
                                var proj = allProjects.Data.FirstOrDefault(s => s.Id == shed.ProjectId);
                                SetEndDate(settings, proj, shed);
                                shed.Order++;
                                await _scheduleRepo.UpdateAsync(shed, false, cancellationTokenSource.Token);
                                nextBDate = shed.EndDate;
                            }
                        }
                        else
                        {
                            schedule.Order = 1;
                        }
                    }
                    else
                    {
                        SetEndDate(settings, project, schedule);
                    }

                    await _scheduleRepo.AddAsync(schedule, false, cancellationTokenSource.Token);
                    return schedule;
                }
                _logger.LogError($"Error in ProjectSelectService::AddProjectToSchedule: no project select in schedule for user: {userId}");
                return null;
            }
            catch (Exception ex)
            {
                await errorNotifyService.Send($"Error in ProjectSelectService:: AddProjectToSchedule: {ex.Message} {ex.StackTrace}");
                _logger.LogError($"Error in ProjectSelectService::AddProjectToSchedule: {ex.Message} {ex.StackTrace}");
                throw;
            }
        }

        private void SetEndDate(UserSettings settings, Project project, Schedule schedule)
        {
            if (project.Period.HasValue)
            {
                schedule.EndDate = schedule.BeginDate.AddMinutes(Math.Max(project.Period.Value + project.AddTime, 0));
            }
            else
            {
                schedule.EndDate = schedule.BeginDate.AddMinutes(settings.DefaultProjectTimespan);
            }
        }
    }


}
