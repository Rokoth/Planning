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
        private static Dictionary<Guid,object> _lockObjects = new Dictionary<Guid, object>();
        private static object _lockObject = new object();
        private static Dictionary<Guid, bool> _editEnables = new Dictionary<Guid, bool>();

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
                await LockUserId(userId);

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(30000);
                var now = DateTimeOffset.Now;
                var _scheduleRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Schedule>>();
                var _projectRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Project>>();
                var userSettingsRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<UserSettings>>();
                              

                await ShiftSchedule(userId, settings, now, true, true);                

                var currentSchedules = (await _scheduleRepo.GetAsync(new DB.Context.Filter<DB.Context.Schedule>()
                {
                    Selector = s => s.UserId == userId && !s.IsClosed
                }, cancellationTokenSource.Token)).Data;

                var userSettings = (await userSettingsRepo.GetAsync(new Filter<UserSettings>()
                {
                    Selector = s => s.UserId == userId
                }, cancellationTokenSource.Token)).Data.FirstOrDefault();

                Schedule nextSchedule = currentSchedules.OrderBy(s=>s.BeginDate).FirstOrDefault();
                var runningSchedule = currentSchedules.FirstOrDefault(s => s.IsRunning);
                if (runningSchedule != null)
                {
                    runningSchedule.IsRunning = false;
                    runningSchedule.IsClosed = true;
                    

                    var currentProject = await _projectRepo.GetAsync(runningSchedule.ProjectId, cancellationTokenSource.Token);
                    if (currentProject != null)
                    {                       
                        if (currentProject.Period.HasValue)
                        {
                            var addTime = GetAddTime(settings, currentProject, runningSchedule);
                            var toUpSchedule = currentSchedules
                                    .Where(s => s.ProjectId == currentProject.Id && s.Id != runningSchedule.Id)
                                    .OrderBy(s => s.BeginDate).FirstOrDefault();
                            
                            if (toUpSchedule != null)
                            {
                                toUpSchedule.AddTime = (toUpSchedule.AddTime??0) + addTime;
                                await _scheduleRepo.UpdateAsync(toUpSchedule, false, cancellationTokenSource.Token);
                            }
                            else
                            {
                                currentProject.AddTime = addTime;
                            }
                            runningSchedule.AddTime = 0;
                        }
                        currentProject.LastUsedDate = now;
                        await _projectRepo.UpdateAsync(currentProject, false, cancellationTokenSource.Token);
                    }
                    await _scheduleRepo.UpdateAsync(runningSchedule, false, cancellationTokenSource.Token);
                    nextSchedule = currentSchedules
                        .Where(s => s.Id!= runningSchedule.Id && s.BeginDate > runningSchedule.BeginDate)
                        .OrderBy(s => s.BeginDate).FirstOrDefault();
                }
                
                if (nextSchedule == null) nextSchedule = await AddProjectToSchedule(userId, userSettings, isLocked: true);
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
            finally
            {
                UnlockUserId(userId);
            }
        }

        private static void UnlockUserId(Guid userId)
        {
            lock (_lockObjects[userId])
            {
                _editEnables[userId] = true;
            }
        }

        private static async Task LockUserId(Guid userId)
        {
            if (!_lockObjects.ContainsKey(userId))
            {
                lock (_lockObject)
                {
                    _lockObjects.Add(userId, new object());
                    _editEnables.Add(userId, true);
                }
            }
            while (true)
            {
                lock (_lockObjects[userId])
                {
                    if (_editEnables[userId])
                    {
                        _editEnables[userId] = false;
                        break;
                    }
                }
                await Task.Delay(10);
            }
        }

        public async Task ShiftSchedule(Guid userId, UserSettings settings, DateTimeOffset now, bool isForce = false, bool isLocked = false)
        {
            try
            {
                if (!isLocked) await LockUserId(userId);
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(30000);
                var _scheduleRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Schedule>>();
                var _projectRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Project>>();
                var userSettingsRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<UserSettings>>();
                var currentSchedules = (await _scheduleRepo.GetAsync(new DB.Context.Filter<DB.Context.Schedule>()
                {
                    Selector = s => s.UserId == userId && !s.IsClosed
                }, cancellationTokenSource.Token)).Data.OrderBy(s => s.BeginDate);

                var beginDate = now;
                var runningSchedule = currentSchedules.FirstOrDefault(s => s.IsRunning);
                if (runningSchedule != null && (isForce || runningSchedule.EndDate < now))
                {
                    foreach (var schedule in currentSchedules.Where(s => s.IsRunning || s.BeginDate > runningSchedule.BeginDate))
                    {
                        var project = await _projectRepo.GetAsync(schedule.ProjectId, cancellationTokenSource.Token);
                        if (schedule.IsRunning)
                        {
                            schedule.EndDate = now;
                        }
                        else
                        {                            
                            schedule.BeginDate = beginDate;
                            SetEndDate(settings, project, schedule);

                            beginDate = schedule.EndDate;
                        }

                        await _scheduleRepo.UpdateAsync(schedule, false, cancellationTokenSource.Token);
                    }
                    await _projectRepo.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                await errorNotifyService.Send($"Error in ProjectSelectService:: ShiftSchedule: {ex.Message} {ex.StackTrace}");
                _logger.LogError($"Error in ProjectSelectService::ShiftSchedule: {ex.Message} {ex.StackTrace}");
                throw;
            }
            finally
            {
                if (!isLocked) UnlockUserId(userId);
            }
        }

        public async Task<Schedule> AddProjectToSchedule(Guid userId, UserSettings settings, Guid? projectId = null, 
            DateTimeOffset? beginDate = null, bool setBeginDate = false, bool isLocked = false)
        {
            try
            {
                if (!isLocked) await LockUserId(userId);
                DB.Context.Project project = null;
                var now = DateTimeOffset.Now;
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(30000);
                var _calculator = _serviceProvider.GetRequiredService<ICalculator>();
                var _projectRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Project>>();
                var _scheduleRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Schedule>>();
                var _userRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.User>>();
                var _formulaRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Formula>>();

                var allSchedules = (await _scheduleRepo.GetAsync(new DB.Context.Filter<DB.Context.Schedule>()
                {
                    Selector = s => s.UserId == userId
                }, cancellationTokenSource.Token)).Data.OrderBy(s => s.BeginDate);

                var currentSchedules = allSchedules.Where(s=>!s.IsClosed);                

                var allProjects = await _projectRepo.GetAsync(
                        new DB.Context.Filter<DB.Context.Project>()
                        {
                            Selector = s => s.UserId == userId
                                && (!settings.LeafOnly || s.IsLeaf)
                        },
                        cancellationTokenSource.Token);

                Schedule lastSchedule = null;
                if(currentSchedules.Any())
                    lastSchedule = currentSchedules.Last();

                if (projectId != null)
                {
                    project = await _projectRepo.GetAsync(projectId.Value, cancellationTokenSource.Token);
                }
                else
                {
                    var user = await _userRepo.GetAsync(userId, cancellationTokenSource.Token);
                    var formula = await _formulaRepo.GetAsync(user.FormulaId, cancellationTokenSource.Token);
                    List<CalcRequestItem> items = new List<CalcRequestItem>();

                    var nowDate = lastSchedule?.EndDate ?? now;
                    if (setBeginDate) nowDate = beginDate.Value;
                    foreach (var item in allProjects.Data)
                    {
                        var fields = JObject.FromObject(item);
                        var lostHours = 0;
                        var prevSched = currentSchedules.Where(s => s.ProjectId == item.Id && s.BeginDate < nowDate).OrderBy(s => s.BeginDate).LastOrDefault();
                        if (prevSched != null)
                        {
                            lostHours = (int)(nowDate - prevSched.EndDate).TotalHours;
                        }
                        else if (item.LastUsedDate.HasValue)
                        {
                            lostHours = (int)(nowDate - item.LastUsedDate.Value).TotalHours;                            
                        }
                        fields.Add("LostHours", lostHours);
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
                        BeginDate = lastSchedule?.EndDate ?? now,
                        Id = Guid.NewGuid(),
                        IsRunning = false,
                        IsDeleted = false,                        
                        ProjectId = project.Id,
                        UserId = userId,
                        VersionDate = DateTimeOffset.Now,
                        IsClosed = false,
                        AddTime = project.AddTime
                    };

                    if (project.AddTime != 0)
                    {
                        project.AddTime = 0;
                        await _projectRepo.UpdateAsync(project, false, cancellationTokenSource.Token);
                    }


                    if (setBeginDate)
                    {
                        var bDate = beginDate ?? now;
                        var toUpdate = currentSchedules.Where(s => s.EndDate > bDate).OrderBy(s => s.BeginDate).ToList();
                        if (toUpdate.Any())
                        {
                            var currShed = toUpdate[0];
                            if ((bDate - currShed.BeginDate).TotalMinutes < 1)
                                bDate = currShed.BeginDate.AddMinutes(1);
                        }
                        schedule.BeginDate = bDate;                        
                        SetEndDate(settings, project, schedule);
                        
                        if (toUpdate.Any())
                        {
                            var currShed = toUpdate[0];
                            currShed.EndDate = bDate;                                                                       
                            await _scheduleRepo.UpdateAsync(currShed, false, cancellationTokenSource.Token);                           
                            var nextBDate = schedule.EndDate;
                            for (int i = 1; i < toUpdate.Count; i++)
                            {
                                var shed = toUpdate[i];
                                shed.BeginDate = nextBDate;                                
                                var proj = allProjects.Data.FirstOrDefault(s => s.Id == shed.ProjectId);                               
                                SetEndDate(settings, proj, shed);                               
                                await _scheduleRepo.UpdateAsync(shed, false, cancellationTokenSource.Token);
                                nextBDate = shed.EndDate;
                            }
                        }                        
                    }
                    else
                    {                        
                        SetEndDate(settings, project, schedule);
                    }

                    await _scheduleRepo.AddAsync(schedule, false, cancellationTokenSource.Token);
                    await _scheduleRepo.SaveChangesAsync();
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
            finally
            {
                if (!isLocked) UnlockUserId(userId);
            }
        }

        private void SetEndDate(UserSettings settings, Project project, Schedule schedule)
        {
            int minutes = GetPeriod(settings, project);
            schedule.EndDate = schedule.BeginDate.AddMinutes(minutes);
        }

        private int GetPeriod(UserSettings settings, Project project)
        {
            var minutes = settings.DefaultProjectTimespan;
            if (project == null) minutes = 1;
            else if (project.Period.HasValue) minutes = project.Period.Value;
            return minutes;
        }

        private int GetAddTime(UserSettings settings, Project project, Schedule schedule)
        {
            if (project.Period.HasValue)
            {
                var standartPeriod = GetPeriod(settings, project);
                var lastUsedDate = project.LastUsedDate;                
                var totalHours = (schedule.EndDate - lastUsedDate).Value.TotalHours;
                var periodHours = 10001 - project.Priority;
                var period = (int)(standartPeriod * (totalHours / periodHours));
                return (schedule.AddTime ?? 0) + period - (int)(schedule.EndDate - schedule.BeginDate).TotalMinutes;                
            }
            return 0;
        }
    }


}
