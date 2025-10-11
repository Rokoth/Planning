using Contracts.Model.Project;
using Contracts.Model.Schedule;
using Contracts.Model.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private readonly IGetDataService<Schedule, ScheduleFilter> _dataService;
        private readonly IGetDataService<User, UserFilter> _userDataService;
        private readonly IUpdateDataService<Schedule, ScheduleUpdater> _updateDataService;
        private readonly IGetDataService<ScheduleHistory, ScheduleHistoryFilter> _historyDataService;
        private readonly IAddDataService<Schedule, ScheduleCreator> _addDataService;
        private readonly IDeleteDataService<Schedule> _deleteDataService;
        private readonly IProjectSelectService _projectSelectService;
        private readonly IRepository<DB.Context.UserSettings> _userSettingsRepository;
        private readonly INotifyDataService _notifyDataService;

        public ScheduleController(ILogger<ScheduleController> logger,
            IGetDataService<Schedule, ScheduleFilter> dataService,
            IGetDataService<User, UserFilter> userDataService,
            IUpdateDataService<Schedule, ScheduleUpdater> updateDataService,
            IGetDataService<ScheduleHistory, ScheduleHistoryFilter> historyDataService,
            IAddDataService<Schedule, ScheduleCreator> addDataService,
            IDeleteDataService<Schedule> deleteDataService,
            IProjectSelectService projectSelectService,
            IRepository<DB.Context.UserSettings> userSettingsRepository,
            INotifyDataService notifyDataService) : base(logger)
        {
            _dataService = dataService;
            _userDataService = userDataService;
            _updateDataService = updateDataService;
            _historyDataService = historyDataService;
            _addDataService = addDataService;
            _deleteDataService = deleteDataService;
            _projectSelectService = projectSelectService;
            _userSettingsRepository = userSettingsRepository;
            _notifyDataService = notifyDataService;
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
               
                CancellationTokenSource source = new CancellationTokenSource(30000);
                Schedule result = await _updateDataService.UpdateAsync(updater, source.Token);
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
               
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _historyDataService.GetAsync(new ScheduleHistoryFilter(size, page, sort, name, id), source.Token);
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return PartialView(result.Data);
            }, "ScheduleController", "HistoryListPaged");
        }

        // GET: UserController/Details/5
        [Authorize]
        public async Task<IActionResult> Details([FromRoute] Guid id)
        {
            return await Execute(async () => {
                
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
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public async Task<IActionResult> Create(ScheduleCreator creator)
        //{
        //    return await Execute(async () => {
        //        var userId = Guid.Parse(User.Identity.Name);                
        //        CancellationTokenSource source = new CancellationTokenSource(30000);
        //        var userSettings = (await _userSettingsRepository.GetAsync(new DB.Context.Filter<DB.Context.UserSettings>()
        //        {
        //            Selector = s => s.UserId == userId
        //        }, source.Token)).Data.Single();

        //        var result = await _projectSelectService.MoveToNextSchedule(userId, userSettings, creator.ProjectId, creator.BeginDate, creator.SetBeginDate);
        //        return RedirectToAction(nameof(Details), new { id = result.Id });
        //    }, "ScheduleController", "Create");
        //}

        // GET: UserController/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            return await Execute(async () => {
                
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
              
                CancellationTokenSource source = new CancellationTokenSource(30000);
                Schedule result = await _deleteDataService.DeleteAsync(id, source.Token);
                return RedirectToAction(nameof(Index));
            }, "ScheduleController", "Delete");
        }

        
        [HttpPost]       
        [Authorize]
        public async Task<IActionResult> MoveNext()
        {
            return await Execute(async () => {
                var userId = Guid.Parse(User.Identity.Name);
           
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var userSettings = (await _userSettingsRepository.GetAsync(new DB.Context.Filter<DB.Context.UserSettings>()
                {
                    Selector = s => s.UserId == userId
                }, source.Token)).Data.Single();

               await _projectSelectService.MoveToNextSchedule(userId);                
                return Ok();
            }, "ScheduleController", "MoveNext");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EndSchedule()
        {
            return await Execute(async () => {
                var userId = Guid.Parse(User.Identity.Name);            
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var userSettings = (await _userSettingsRepository.GetAsync(new DB.Context.Filter<DB.Context.UserSettings>()
                {
                    Selector = s => s.UserId == userId
                }, source.Token)).Data.Single();

                await _projectSelectService.MoveToNextSchedule(userId);
                return Ok();
            }, "ScheduleController", "EndSchedule");
        }
        

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GetCurrentNotify()
        {
            return await Execute(async () => {
                var userId = Guid.Parse(User.Identity.Name);               

                var nextNotify = (await _notifyDataService.GetNotifiesAsync(userId)).OrderBy(s => s.VersionDate).FirstOrDefault();
                if(nextNotify != null)
                {
                    await _notifyDataService.SetNotifySend(nextNotify.Id);
                }
                return Ok(nextNotify?.Text);
            }, "ScheduleController", "GetCurrentNotify");
        }
    }
}
