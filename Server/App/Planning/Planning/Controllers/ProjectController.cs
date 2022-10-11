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
    public class ProjectController : CommonControllerBase
    {        
        public ProjectController(IServiceProvider serviceProvider):base(serviceProvider)
        {            
        }

        // GET: UserController
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return await Execute(async () => {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new ProjectFilter(userId, null, null, null, null, null,
                    null, null, null, null), source.Token);               
                return View(result.Data);
            }, "ProjectController", "Index");
        }

        [Authorize]
        public async Task<IActionResult> IndexChilds(Guid parentId)
        {
            return await Execute(async () => {
                var userId = Guid.Parse(User.Identity.Name);                            
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);                
                var result = await _dataService.GetAsync(new ProjectFilter(userId, null, null, null, null, null,
                    null, null, parentId, null), source.Token);                
                
                return PartialView(result.Data);
            }, "ProjectController", "IndexChilds");
        }

        [Authorize]
        public async Task<IActionResult> ListSelectChilds(Guid parentId)
        {
            return await Execute(async () => {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                var _userDataService = _serviceProvider.GetRequiredService<IGetDataService<User, UserFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new ProjectFilter(userId, null, null, null, null, null,
                    null, null, parentId, null), source.Token);
                var user = await _userDataService.GetAsync(userId, source.Token);
                var ret = result.Data.ToList();
                foreach (var item in ret)
                {
                    item.CanSelect = item.IsLeaf || !user.LeafOnly;
                }
                return PartialView(ret);
            }, "ProjectController", "ListSelectChilds");
        }

        // GET: UserController/Details/5
        [Authorize]
        public async Task<IActionResult> Details([FromRoute] Guid id)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                var cancellationTokenSource = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(id, cancellationTokenSource.Token);
                return View(result);
            }, "ProjectController", "Details");
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
            return await Execute(async () => {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new ProjectFilter(userId, null, null, null, null, null,
                    null, null, null, null), source.Token);               
                return PartialView(result.Data);
            }, "ProjectController", "ListSelect");
        }               

        // GET: ClientController/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            return await Execute(async () => {
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
            }, "ProjectController", "Edit");
        }

        // POST: ClientController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id, ProjectUpdater updater)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IUpdateDataService<Project, ProjectUpdater>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                Project result = await _dataService.UpdateAsync(updater, source.Token);
                return RedirectToAction("Details", new { id = result.Id });
            }, "ProjectController", "Edit");
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
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<ProjectHistory, ProjectHistoryFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new ProjectHistoryFilter(size, page, sort, name, null, null, id, userId), source.Token);
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return PartialView(result.Data);
            }, "ProjectController", "HistoryListPaged");
        }               

        // GET: UserController/Create
        [Authorize]
        public async Task<IActionResult> Create(Guid? parentId)
        {
            return await Execute(async () => {
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
            }, "ProjectController", "Create");
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(ProjectCreator creator)
        {
            return await Execute(async () => {
                creator.IsLeaf = true;
                creator.LastUsedDate = DateTimeOffset.Now;
                var _dataService = _serviceProvider.GetRequiredService<IAddDataService<Project, ProjectCreator>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                Project result = await _dataService.AddAsync(creator, source.Token);
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }, "ProjectController", "Create");
        }

        // GET: UserController/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(id, source.Token);
                return View(result);
            }, "ProjectController", "Delete");
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id, Project model)
        {
            return await Execute(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IDeleteDataService<Project>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.DeleteAsync(id, source.Token);
                return RedirectToAction(nameof(Index));
            }, "ProjectController", "Delete");
        }
    }
}
