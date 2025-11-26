using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.Contracts.Model;
using Planning.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Controllers
{
    [Route("api/v1/projects")]
    [Produces("application/json")]
    [ApiController]
    public class ProjectApiController : CommonControllerBase
    {
        private IGetDataService<Project, ProjectFilter> _dataService;
        private IUpdateDataService<Project, ProjectUpdater> _updateDataService;
        private IAddDataService<Project, ProjectCreator> _addDataService;
        private IGetDataService<ProjectHistory, ProjectHistoryFilter> _historyDataService;
        public ProjectApiController(ILogger<ProjectApiController> logger,
            IGetDataService<Project, ProjectFilter> dataService,
            IUpdateDataService<Project, ProjectUpdater> updateDataService,
            IAddDataService<Project, ProjectCreator> addDataService,
            IGetDataService<ProjectHistory, ProjectHistoryFilter> historyDataService) : base(logger)
        {
           
        }

        [HttpGet]
        [Authorize("Token")]
        public async Task<IActionResult> Get(string name = null, string path = null, int size = 10, 
            int page = 0, string sort = null, bool? isLeaf = null, DateTimeOffset? lastUsedDateBegin = null, DateTimeOffset? lastUsedDateEnd = null, Guid? parentId = null)
        {
            return await ExecuteApi(async () => {
                var cur = ClaimsPrincipal.Current;
                var userId = Guid.Parse(User.Identity.Name);               
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new ProjectFilter(userId, size, page, sort, name, isLeaf,
                    lastUsedDateBegin, lastUsedDateEnd, parentId, path), source.Token);                
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return Ok(result.Data);
            }, "ProjectApiController", "Get");
        }

        [HttpGet("{id}")]
        [Authorize("Token")]
        public async Task<IActionResult> GetItem([FromRoute] Guid id)
        {
            return await ExecuteApi(async () => {
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(id, source.Token);
                return Ok(result);
            }, "ProjectApiController", "GetItem");
        }
    }
}
