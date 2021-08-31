using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Planning.Contract.Model;
using Planning.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Controllers
{
    public class UserController : CommonControllerBase
    {
        
        public UserController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // GET: UserController
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> ListPaged(int page = 0, int size = 10, string sort = null, string name = null)
        {
            return await ExecuteApi(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<User, UserFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new UserFilter(size, page, sort, name), source.Token);
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return PartialView(result.Data);
            }, "UserController", "ListPaged");
        }

        // GET: UserController
        [Authorize]
        public ActionResult History()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> HistoryListPaged(int page = 0, int size = 10, string sort = null, string name = null, Guid? id = null)
        {
            return await ExecuteApi(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<UserHistory, UserHistoryFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new UserHistoryFilter(size, page, sort, name, id), source.Token);
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return PartialView(result.Data);
            }, "UserController", "HistoryListPaged");
        }

        // GET: UserController/Details/5
        [Authorize]
        public async Task<IActionResult> Details(Guid id)
        {
            return await ExecuteApi(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<User, UserFilter>>();
                var cancellationTokenSource = new CancellationTokenSource(30000);
                User result = await _dataService.GetAsync(id, cancellationTokenSource.Token);
                return View(result);
            }, "UserController", "Details");
        }

        // GET: UserController/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            return await ExecuteApi(async () => {
                var _formulaDataService = _serviceProvider.GetRequiredService<IGetDataService<Formula, FormulaFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var defaultFormula = (await _formulaDataService.GetAsync(new FormulaFilter(1, 0, null, null, true), source.Token)).Data.FirstOrDefault();
                if (defaultFormula == null)
                {
                    defaultFormula = (await _formulaDataService.GetAsync(new FormulaFilter(1, 0, null, null, null), source.Token)).Data.FirstOrDefault();
                }
                var user = new UserCreator()
                {
                    FormulaId = defaultFormula.Id,
                    Formula = defaultFormula.Name
                };
                return View(user);
            }, "UserController", "Create");
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(UserCreator creator)
        {
            return await ExecuteApi(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IAddDataService<User, UserCreator>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                User result = await _dataService.AddAsync(creator, source.Token);
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }, "UserController", "Create");
        }

        // GET: UserController/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            return await ExecuteApi(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<User, UserFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                User result = await _dataService.GetAsync(id, source.Token);
                var updater = new UserUpdater()
                {
                    Description = result.Description,
                    Id = result.Id,
                    Login = result.Login,
                    Name = result.Name,
                    PasswordChanged = false,
                    ScheduleTimeSpan = result.ScheduleTimeSpan,
                    ScheduleShift = result.ScheduleShift,
                    ScheduleMode = result.ScheduleMode,
                    ScheduleCount = result.ScheduleCount,
                    LeafOnly = result.LeafOnly,
                    Formula = result.Formula,
                    DefaultProjectTimespan = result.DefaultProjectTimespan,
                    FormulaId = result.FormulaId
                };
                return View(updater);
            }, "UserController", "Edit");
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id, UserUpdater updater)
        {
            return await ExecuteApi(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IUpdateDataService<User, UserUpdater>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                User result = await _dataService.UpdateAsync(updater, source.Token);
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }, "UserController", "Edit");
        }

        // GET: UserController/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            return await ExecuteApi(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<User, UserFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                User result = await _dataService.GetAsync(id, source.Token);
                return View(result);
            }, "UserController", "Delete");
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id, User model)
        {
            return await ExecuteApi(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IDeleteDataService<User>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                User result = await _dataService.DeleteAsync(id, source.Token);
                return RedirectToAction(nameof(Index));
            }, "UserController", "Delete");
        }
    }
}
