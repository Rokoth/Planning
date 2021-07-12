using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
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

        public ProjectSelectService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<ProjectSelectService>>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
        }

        public async Task<Contract.Model.Project> AddProjectToSchedule(Guid userId, bool leafOnly, DateTimeOffset beginDate, Guid? projectId = null)
        {
            DB.Context.Project project = null;
            DB.Context.Project currentProject = null;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(30000);
            var _calculator = _serviceProvider.GetRequiredService<ICalculator>();
            var _projectRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Project>>();
            var _scheduleRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Schedule>>();
            var _userRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.User>>();
            var _formulaRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.Formula>>();

            var lastSchedule = (await _scheduleRepo.GetAsync(new DB.Context.Filter<DB.Context.Schedule>()
            {
                Page = 0,
                Size = 1,
                Sort = "Order desc",
                Selector = s => s.UserId == userId
            }, cancellationTokenSource.Token)).Data.FirstOrDefault();

            if (lastSchedule != null)
            {
                currentProject = await _projectRepo.GetAsync(lastSchedule.ProjectId, cancellationTokenSource.Token);
            }

            if (projectId != null)
            {
                project = await _projectRepo.GetAsync(projectId.Value, cancellationTokenSource.Token);
            }
            else
            {
                var user = await _userRepo.GetAsync(userId, cancellationTokenSource.Token);
                var formula = await _formulaRepo.GetAsync(user.FormulaId, cancellationTokenSource.Token);
                List<CalcRequestItem> items = new List<CalcRequestItem>();
                var allProjects = await _projectRepo.GetAsync(
                    new DB.Context.Filter<DB.Context.Project>()
                    {
                        Selector = s => s.UserId == userId
                            && (!leafOnly || s.IsLeaf)
                    },
                    cancellationTokenSource.Token);

                foreach (var item in allProjects.Data)
                {
                    var fields = JObject.FromObject(item);
                    if (item.LastUsedDate.HasValue)
                    {
                        var lostHours = ((beginDate - item.LastUsedDate.Value).TotalHours);
                        fields.Add("LostHours", lostHours);
                    }
                    var request = new CalcRequestItem()
                    {
                        Id = item.Id,
                        Fields = fields.ToString()
                    };
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
                if (lastSchedule.IsRunning)
                {
                    lastSchedule.IsRunning = false;
                    lastSchedule.ElapsedTime += (int)(DateTimeOffset.Now - lastSchedule.StartDate).TotalMinutes;
                    await _scheduleRepo.UpdateAsync(lastSchedule, true, cancellationTokenSource.Token);
                }

                project.LastUsedDate = beginDate;
                await _projectRepo.UpdateAsync(project, true, cancellationTokenSource.Token);
                if (currentProject.Period.HasValue) currentProject.AddTime += (currentProject.Period.Value - lastSchedule.ElapsedTime);
                await _projectRepo.UpdateAsync(currentProject, true, cancellationTokenSource.Token);

                var schedule = new DB.Context.Schedule()
                {
                    BeginDate = beginDate,
                    ElapsedTime = 0,
                    Id = Guid.NewGuid(),
                    IsRunning = false,
                    IsDeleted = false,
                    Order = lastSchedule?.Order ?? 1,
                    ProjectId = project.Id,
                    UserId = userId,
                    VersionDate = DateTimeOffset.Now
                };

                if (project.Period.HasValue)
                {
                    schedule.EndDate = beginDate.AddMinutes(Math.Max(project.Period.Value + project.AddTime, 0));
                }

                await _scheduleRepo.AddAsync(schedule, true, cancellationTokenSource.Token);
            }
            return _mapper.Map<Contract.Model.Project>(project);
        }
    }


}
