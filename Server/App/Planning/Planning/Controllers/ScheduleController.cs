using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.Contract.Model;
using Planning.DB.Repository;
using Planning.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Controllers
{
    public class ScheduleController : CommonControllerBase
    {      
        public ScheduleController(IServiceProvider serviceProvider) : base(serviceProvider)
        {          
        }

        // GET: UserController
        [Authorize]

        public ActionResult Index()
        {            
            return View();
        }

        [Authorize]
        public async Task<IActionResult> ListPaged([FromQuery]int page = 0, [FromQuery]int size = 10,
            [FromQuery]string sort = null, [FromQuery]string name = null, [FromQuery] bool? onlyActive = null
            , [FromQuery] DateTimeOffset? fromDate = null, [FromQuery] DateTimeOffset? toDate = null)
        {
            return await Execute(async () => {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Schedule, ScheduleFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                                
                var result = await _dataService.GetAsync(new ScheduleFilter(size, page, sort, name, null, 
                    userId, onlyActive, fromDate, toDate), source.Token);
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return PartialView(result.Data);
            }, "ScheduleController", "ListPaged");
        }

        [Authorize]

        public ActionResult ListSelect()
        {
            return PartialView();
        }

        [Authorize]
        public async Task<IActionResult> ListSelectPaged([FromQuery] int page = 0, [FromQuery] int size = 10,
            [FromQuery] string sort = null, [FromQuery] string name = null)
        {
            return await Execute(async () => {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Schedule, ScheduleFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new ScheduleFilter(size, page, sort, name, null, userId), source.Token);
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return PartialView(result.Data);
            }, "ScheduleController", "ListSelectPaged");
        }

        // GET: ClientController/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Schedule, ScheduleFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                Schedule result = await _dataService.GetAsync(id, source.Token);
                var updater = new ScheduleUpdater()
                {                    
                    Id = result.Id,
                    ProjectId = result.ProjectId,
                    BeginDate = result.BeginDate
                };
                return View(updater);
            }, "ScheduleController", "Edit");
        }

        // POST: ClientController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id, ScheduleUpdater updater)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IUpdateDataService<Schedule, ScheduleUpdater>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                Schedule result = await _dataService.UpdateAsync(updater, source.Token);
                return RedirectToAction("Details", new { id = result.Id });
            }, "ScheduleController", "Edit");
        }

        // GET: UserController
        [Authorize]
        public ActionResult History()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> HistoryListPaged([FromRoute] Guid id, [FromQuery] int page = 0, [FromQuery] int size = 10,
            [FromQuery] string sort = null, [FromQuery] string name = null)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<ScheduleHistory, ScheduleHistoryFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new ScheduleHistoryFilter(size, page, sort, name, id), source.Token);
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return PartialView(result.Data);
            }, "ScheduleController", "HistoryListPaged");
        }

        // GET: UserController/Details/5
        [Authorize]
        public async Task<IActionResult> Details([FromRoute] Guid id)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Schedule, ScheduleFilter>>();
                var cancellationTokenSource = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(id, cancellationTokenSource.Token);
                return View(result);
            }, "ScheduleController", "Details");
        }

        // GET: UserController/Create
        [Authorize]
        public ActionResult Create()
        {
            var userId = Guid.Parse(User.Identity.Name);
            //Fill default fields
            var entity = new ScheduleCreator()
            {
                UserId = userId,
                BeginDate = DateTimeOffset.Now,
                SetBeginDate = false
            };
            return View(entity);
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(ScheduleCreator creator)
        {
            return await Execute(async () => {
                var userId = Guid.Parse(User.Identity.Name);
                var selectService = _serviceProvider.GetRequiredService<IProjectSelectService>();
                var userSettingsRepo = _serviceProvider.GetRequiredService<IRepository<DB.Context.UserSettings>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var userSettings = (await userSettingsRepo.GetAsync(new DB.Context.Filter<DB.Context.UserSettings>()
                {
                    Selector = s => s.UserId == userId
                }, source.Token)).Data.Single();

                var result = await selectService.AddProjectToSchedule(userId, userSettings, creator.ProjectId, creator.BeginDate, creator.SetBeginDate);
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }, "ScheduleController", "Create");
        }

        // GET: UserController/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Schedule, ScheduleFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                Schedule result = await _dataService.GetAsync(id, source.Token);
                return View(result);
            }, "ScheduleController", "Delete");
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id, Schedule model)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IDeleteDataService<Schedule>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                Schedule result = await _dataService.DeleteAsync(id, source.Token);
                return RedirectToAction(nameof(Index));
            }, "ScheduleController", "Delete");
        }

        
        [HttpPost]       
        [Authorize]
        public async Task<IActionResult> MoveNext()
        {
            return await Execute(async () => {
                var userId = Guid.Parse(User.Identity.Name);
                var selectService = _serviceProvider.GetRequiredService<IProjectSelectService>();
                var userSettingsRepo = _serviceProvider.GetRequiredService<IRepository<DB.Context.UserSettings>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var userSettings = (await userSettingsRepo.GetAsync(new DB.Context.Filter<DB.Context.UserSettings>()
                {
                    Selector = s => s.UserId == userId
                }, source.Token)).Data.Single();

               await selectService.MoveNextSchedule(userId, userSettings);                
                return Ok();
            }, "ScheduleController", "MoveNext");
        }
    }
}
