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
    public class ProjectController : Controller
    {
        private IServiceProvider _serviceProvider;

        public ProjectController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // GET: UserController
        [Authorize]
        public async Task<ActionResult> Index()
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new ProjectFilter(userId, null, null, null, null, null,
                    null, null, null, null), source.Token);               
                return View(result.Data);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { Message = ex.Message });
            }
        }

        [Authorize]
        public async Task<ActionResult> IndexChilds(Guid parentId)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new ProjectFilter(userId, null, null, null, null, null,
                    null, null, parentId, null), source.Token);
                return PartialView(result.Data);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { Message = ex.Message });
            }
        }

        [Authorize]
        public async Task<ActionResult> ListSelectChilds(Guid parentId)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new ProjectFilter(userId, null, null, null, null, null,
                    null, null, parentId, null), source.Token);
                return PartialView(result.Data);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { Message = ex.Message });
            }
        }

        // GET: UserController/Details/5
        [Authorize]
        public async Task<ActionResult> Details([FromRoute] Guid id)
        {
            try
            {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                var cancellationTokenSource = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(id, cancellationTokenSource.Token);
                return View(result);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { Message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<JsonResult> CheckName(string name, Guid? parentId)
        {
            var userId = Guid.Parse(User.Identity.Name);
            bool result = false;
            if (!string.IsNullOrEmpty(name))
            {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var check = await _dataService.GetAsync(new ProjectFilter(userId, null, null, null, name, null, null, null, parentId, null), source.Token);
                result = !check.Data.Where(s=>s.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).Any();
            }
            return Json(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<JsonResult> CheckNameEdit(string name, Guid? parentId, Guid id)
        {
            var userId = Guid.Parse(User.Identity.Name);
            bool result = false;
            if (!string.IsNullOrEmpty(name))
            {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var check = await _dataService.GetAsync(new ProjectFilter(userId, null, null, null, name, null, null, null, parentId, null), source.Token);
                result = !check.Data.Where(s=> s.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && s.Id!=id).Any();
            }
            return Json(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<JsonResult> CheckPath(string path, Guid? parentId)
        {
            var userId = Guid.Parse(User.Identity.Name);
            bool result = false;
            if (!string.IsNullOrEmpty(path))
            {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var check = await _dataService.GetAsync(new ProjectFilter(userId, null, null, null, null, null, null, null, parentId, path), source.Token);
                result = !check.Data.Where(s => s.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase)).Any();
            }
            return Json(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<JsonResult> CheckPathEdit(string path, Guid? parentId, Guid id)
        {
            var userId = Guid.Parse(User.Identity.Name);
            bool result = false;
            if (!string.IsNullOrEmpty(path))
            {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var check = await _dataService.GetAsync(new ProjectFilter(userId, null, null, null, null, null, null, null, parentId, path), source.Token);
                result = !check.Data.Where(s => s.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase) && s.Id != id).Any();
            }
            return Json(result);
        }

        [Authorize]

        public async Task<IActionResult> ListSelect()
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new ProjectFilter(userId, null, null, null, null, null,
                    null, null, null, null), source.Token);               
                return PartialView(result.Data);
            }
            catch (Exception ex)
            {                
                return RedirectToAction("Error", $"Method ProjectController::ListSelect exception: {ex.Message} + ST: {ex.StackTrace}");
            }
        }               

        // GET: ClientController/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                Project result = await _dataService.GetAsync(id, source.Token);
                if(result.UserId!= userId) return RedirectToAction("Index", "Error", new { Message = "Проект привязан к другому пользователю" });
                var updater = new ProjectUpdater()
                {
                    Id = result.Id,
                    Name = result.Name,
                   AddTime = result.AddTime,
                   IsLeaf = result.IsLeaf,
                   LastUsedDate = result.LastUsedDate,
                   Parent = result.Parent,
                   ParentId = result.ParentId,
                   Path = result.Path,
                   Period = result.Period,
                   Priority = result.Priority,
                   UserId = userId
                };
                return View(updater);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { Message = ex.Message });
            }
        }

        // POST: ClientController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit(Guid id, ProjectUpdater updater)
        {
            try
            {
                var _dataService = _serviceProvider.GetRequiredService<IUpdateDataService<Project, ProjectUpdater>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                Project result = await _dataService.UpdateAsync(updater, source.Token);
                return RedirectToAction("Details", new { id = result.Id });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { Message = ex.Message });
            }
        }

        // GET: UserController
        [Authorize]
        public ActionResult History()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> HistoryListPaged([FromRoute] Guid id, [FromQuery] int page = 0, [FromQuery] int size = 10,
            [FromQuery] string sort = null, [FromQuery] string name = null)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<ProjectHistory, ProjectHistoryFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new ProjectHistoryFilter(size, page, sort, name, null, null, id, userId), source.Token);
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return PartialView(result.Data);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { Message = ex.Message });
            }
        }               

        // GET: UserController/Create
        [Authorize]
        public async Task<ActionResult> Create(Guid? parentId)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var project = new ProjectCreator() { 
                   UserId = userId,
                   IsLeaf = true
                };
                if (parentId.HasValue)
                {
                    var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                    CancellationTokenSource source = new CancellationTokenSource(30000);
                    var parent = await _dataService.GetAsync(parentId.Value, source.Token);
                    project.ParentId = parentId;
                    project.Parent = parent.Name;
                }                               
                
                return View(project);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { Message = ex.Message });
            }         
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create(ProjectCreator creator)
        {
            try
            {
                creator.IsLeaf = true;
                var _dataService = _serviceProvider.GetRequiredService<IAddDataService<Project, ProjectCreator>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                Project result = await _dataService.AddAsync(creator, source.Token);
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { Message = ex.Message });
            }
        }

        // GET: UserController/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(id, source.Token);
                return View(result);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { Message = ex.Message });
            }
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Delete(Guid id, Project model)
        {
            try
            {
                var _dataService = _serviceProvider.GetRequiredService<IDeleteDataService<Project>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.DeleteAsync(id, source.Token);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { Message = ex.Message });
            }
        }
    }
}
