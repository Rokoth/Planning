using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.Contract.Model;
using Planning.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Controllers
{
    public class FormulaController : CommonControllerBase
    {       
        public FormulaController(IServiceProvider serviceProvider): base(serviceProvider)
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
            [FromQuery]string sort = null, [FromQuery]string name = null)
        {
            return await Execute(async ()=> {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Formula, FormulaFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new FormulaFilter(size, page, sort, name, null), source.Token);
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return PartialView(result.Data);
            }, "FormulaController", "ListPaged");
        }

        [HttpGet]
        public async Task<JsonResult> CheckName(string name)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(name))
            {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Formula, FormulaFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var check = await _dataService.GetAsync(new FormulaFilter(10, 0, null, name, null), source.Token);
                result = !check.Data.Any();
            }
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> CheckNameEdit(string name, Guid id)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(name))
            {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Formula, FormulaFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var check = await _dataService.GetAsync(new FormulaFilter(10, 0, null, name, null), source.Token);
                result = !check.Data.Where(s=>s.Id!=id).Any();
            }
            return Json(result);
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
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Formula, FormulaFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new FormulaFilter(size, page, sort, name, null), source.Token);
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return PartialView(result.Data);
            }, "FormulaController", "ListSelectPaged");
        }

        // GET: ClientController/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Formula, FormulaFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                Formula result = await _dataService.GetAsync(id, source.Token);
                var updater = new FormulaUpdater()
                {                    
                    Id = result.Id,
                    Name = result.Name,
                    IsDefault = result.IsDefault,
                    Text = result.Text
                };
                return View(updater);
            }, "FormulaController", "ListSelectPaged");
        }

        // POST: ClientController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id, FormulaUpdater updater)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IUpdateDataService<Formula, FormulaUpdater>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                Formula result = await _dataService.UpdateAsync(updater, source.Token);
                return RedirectToAction("Details", new { id = result.Id });
            }, "FormulaController", "ListSelectPaged");
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
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<FormulaHistory, FormulaHistoryFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new FormulaHistoryFilter(id, size, page, sort, name), source.Token);
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return PartialView(result.Data);
            }, "FormulaController", "HistoryListPaged");
        }

        // GET: UserController/Details/5
        [Authorize]
        public async Task<IActionResult> Details([FromRoute] Guid id)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Formula, FormulaFilter>>();
                var cancellationTokenSource = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(id, cancellationTokenSource.Token);
                return View(result);
            }, "FormulaController", "Details");
        }

        // GET: UserController/Create
        [Authorize]
        public ActionResult Create()
        {
            //Fill default fields
            var user = new FormulaCreator();
            return View(user);
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(FormulaCreator creator)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IAddDataService<Formula, FormulaCreator>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);                
                Formula result = await _dataService.AddAsync(creator, source.Token);
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }, "FormulaController", "Create");
        }

        // GET: UserController/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Formula, FormulaFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                Formula result = await _dataService.GetAsync(id, source.Token);
                return View(result);
            }, "FormulaController", "Delete");
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id, Formula model)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IDeleteDataService<Formula>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                Formula result = await _dataService.DeleteAsync(id, source.Token);
                return RedirectToAction(nameof(Index));
            }, "FormulaController", "Delete");
        }
    }
}
