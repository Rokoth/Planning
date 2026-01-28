using Antlr4.Runtime;
using AutoMapper;
using Contracts.Model.Project;
using Contracts.Model.Schedule;
using Contracts.Model.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Planning.Common;
using Planning.DB.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
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
        private readonly IErrorNotifyService _errorNotifyService;
        private static Dictionary<Guid,object> _lockObjects = new Dictionary<Guid, object>();
        private static object _lockObject = new object();
        private static Dictionary<Guid, bool> _editEnables = new Dictionary<Guid, bool>();
        private readonly DB.Repository.IRepository<DB.Context.Schedule> _scheduleRepo;
        private readonly DB.Repository.IRepository<DB.Context.Project> _projectRepo;
        private readonly DB.Repository.IRepository<DB.Context.UserSettings> _userSettingsRepo;

        private readonly DB.Repository.IRepository<DB.Context.Direction> _directionRepo;
        private readonly DB.Repository.IRepository<DB.Context.DirectionCategory> _directionCategoryRepo;
        private readonly DB.Repository.IRepository<DB.Context.DirectionProject> _directionProjectRepo;

        public ProjectSelectService(IServiceProvider serviceProvider,
            DB.Repository.IRepository<DB.Context.Schedule> scheduleRepo, 
            DB.Repository.IRepository<DB.Context.Project> projectRepo, 
            DB.Repository.IRepository<DB.Context.UserSettings> userSettingsRepo,
            DB.Repository.IRepository<DB.Context.Direction> directionRepo,
            DB.Repository.IRepository<DB.Context.DirectionCategory> directionCategoryRepo,
            DB.Repository.IRepository<DB.Context.DirectionProject> directionProjectRepo,
            IErrorNotifyService errorNotifyService,
            IMapper mapper, ILogger<ProjectSelectService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _mapper = mapper;
            _errorNotifyService = errorNotifyService;           
            _scheduleRepo = scheduleRepo;
            _projectRepo = projectRepo;
            _userSettingsRepo = userSettingsRepo;
            _directionRepo = directionRepo;
            _directionCategoryRepo = directionCategoryRepo;
            _directionProjectRepo = directionProjectRepo;
        }

        public async Task<Contracts.Model.Schedule.Schedule> MoveToNextSchedule(Guid userId, Guid? projectId = null, Guid? directionId = null, DateTimeOffset? beginDate = null)
        {
            try
            {
                await LockUserId(userId);

                CancellationTokenSource cancellationTokenSource = new(30000);
                var now = DateTimeOffset.Now;
                var token = cancellationTokenSource.Token;


                var runningSchedule = (await _scheduleRepo.GetAsync(new DB.Context.Filter<DB.Context.Schedule>()
                {
                    Selector = s => s.UserId == userId && s.IsRunning
                }, token)).Data.FirstOrDefault();                

                runningSchedule.IsRunning = false;
                runningSchedule.IsClosed = true;
                runningSchedule.EndDate = now;
                await _scheduleRepo.UpdateAsync(runningSchedule, false, token);

                var schPeriod = (runningSchedule.EndDate - runningSchedule.BeginDate).TotalMinutes;

                var allCategories = (await _directionCategoryRepo.GetAsync(new Filter<DirectionCategory>() { Selector = s => !s.IsDeleted && s.UserId == userId }, token)).Data;
                var allDirections = (await _directionRepo.GetAsync(new Filter<Direction>() { Selector = s => !s.IsDeleted && s.UserId == userId }, token)).Data;
                var runningDirection = allDirections.FirstOrDefault(s => s.Id == runningSchedule.DirectionId);
                runningDirection.Priority -= (int)Math.Ceiling(schPeriod);
                foreach(var direct in allDirections)
                {
                    var category = allCategories.FirstOrDefault(s => s.Id == direct.DirectionCategoryId);
                    direct.Priority += (decimal)(category.Priority * (schPeriod / 60));
                    await _directionRepo.UpdateAsync(direct, false, token);
                }

                var allProjects = (await _projectRepo.GetAsync(new Filter<DB.Context.Project>()
                {
                    Selector = s => !s.IsDeleted
                    && s.UserId == userId
                    && s.IsLeaf
                }, token)).Data;

                var currentProject = allProjects.FirstOrDefault(s => s.Id == runningSchedule.ProjectId);
                var allSchedules = await _scheduleRepo.GetAsync(new Filter<DB.Context.Schedule>() { 
                    Selector = s => s.ProjectId == runningSchedule.ProjectId && s.IsRunning == false
                }, token);
                var avgPeriod = Math.Ceiling((decimal)(allSchedules.Data.Sum(s => (s.EndDate - s.BeginDate).TotalMinutes) + schPeriod) /
                    (decimal)(allSchedules.Data.Count() + 1)) + 1;

                currentProject.Period = (int)avgPeriod;
                currentProject.Priority -= (int)Math.Ceiling(schPeriod * (currentProject.Priority / 5000));
                currentProject.LastUsedDate = now;
                await _projectRepo.UpdateAsync(currentProject, false, token);
                
                var delta = allProjects.Average(s => s.Priority) - 5000;

                if (Math.Abs(delta / allProjects.Count()) > 1)
                {
                    foreach(var proj in allProjects)
                    {
                        proj.Priority += (int)(delta / allProjects.Count());
                        await _projectRepo.UpdateAsync(proj, false, token);
                    }
                }

                var nextSchedule = await GetNextShedule(allDirections, allProjects, projectId, directionId, now);
                nextSchedule.IsRunning = true;
                await _scheduleRepo.AddAsync(new DB.Context.Schedule()
                {
                    BeginDate = nextSchedule.BeginDate,
                    DirectionId = nextSchedule.DirectionId,
                    EndDate = nextSchedule.EndDate,
                    Id = nextSchedule.Id,
                    IsClosed = false,
                    IsDeleted = false,
                    IsRunning = true,
                    ProjectId = nextSchedule.ProjectId,
                    UserId = userId,
                    VersionDate = DateTimeOffset.Now
                }, false, token);
                await _scheduleRepo.SaveChangesAsync();

                var project = await _projectRepo.GetAsync(nextSchedule.ProjectId, token);
                return nextSchedule;
            }
            catch (Exception ex)
            {
                await _errorNotifyService.Send($"Error in ProjectSelectService:: MoveNextSchedule: {ex.Message} {ex.StackTrace}");
                _logger.LogError($"Error in ProjectSelectService:: MoveNextSchedule: {ex.Message} {ex.StackTrace}");
                throw;
            }
            finally
            {
                UnlockUserId(userId);
            }
        }

        public async Task<IEnumerable<Contracts.Model.Schedule.Schedule>> GetNextShedules(
            Guid userId,                   
            int count,
            DateTimeOffset? beginDate,
            CancellationToken token)
        {
            var directions = (await _directionRepo.GetAsync(new Filter<Direction>() 
            { 
                Selector = s => !s.IsDeleted && s.UserId == userId 
            }, token)).Data;
            var projects = (await _projectRepo.GetAsync(new Filter<DB.Context.Project>()
            {
                Selector = s => !s.IsDeleted
                && s.UserId == userId
                && s.IsLeaf
            }, token)).Data;

            List<Contracts.Model.Schedule.Schedule> result = new();
            var intBeginDate = beginDate ?? DateTimeOffset.Now;
            for (int i = 0; i< count; i++)
            {
                var next = await GetNextShedule(directions, projects, null, null, intBeginDate);
                intBeginDate = next.EndDate;
                result.Add(next);
            }
            return result;
        }

        private async Task<Contracts.Model.Schedule.Schedule> GetNextShedule(            
            IEnumerable<Direction> directions,
            IEnumerable<DB.Context.Project> projects,
            Guid? projectId,
            Guid? directionId,
            DateTimeOffset beginDate)
        {
            //todo
            try
            {
                if(projectId != null)
                {


                    return new Contracts.Model.Schedule.Schedule()
                    {

                    };
                }

                if(directionId == null)
                {
                    directionId = directions.OrderByDescending(s => s.Priority).FirstOrDefault().Id;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<DB.Context.Schedule> GetNextProjectSchedule(Guid userId, 
            IEnumerable<Direction> directions,
            IEnumerable<DB.Context.Project> projects,
            Guid? projectId,
            Guid? directionId)
        {
            try
            {               
                CancellationTokenSource cancellationTokenSource = new(30000);
                var token = cancellationTokenSource.Token;
                
                var userSettings = (await _userSettingsRepo.GetAsync(new Filter<DB.Context.UserSettings>()
                {
                    Selector = s => s.UserId == userId
                }, token)).Data.FirstOrDefault();

                DB.Context.Project project = null;
                var now = DateTimeOffset.Now;
                               
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
                                && (!userSettings.LeafOnly || s.IsLeaf)
                        },
                        cancellationTokenSource.Token);

                DB.Context.Schedule lastSchedule = null;
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
                    var schedule = new DB.Context.Schedule()
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
                await _errorNotifyService.Send($"Error in ProjectSelectService:: AddProjectToSchedule: {ex.Message} {ex.StackTrace}");
                _logger.LogError($"Error in ProjectSelectService::AddProjectToSchedule: {ex.Message} {ex.StackTrace}");
                throw;
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


    }


}
