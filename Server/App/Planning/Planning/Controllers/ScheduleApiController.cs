using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Route("api/v1/shedules")]
    [Produces("application/json")]
    [ApiController]
    public class ScheduleApiController : CommonControllerBase
    {       
        public ScheduleApiController(IServiceProvider serviceProvider): base(serviceProvider)
        {
           
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(string name = null, string path = null, int size = 10, 
            int page = 0, string sort = null, bool? isLeaf = null, DateTimeOffset? lastUsedDateBegin = null, DateTimeOffset? lastUsedDateEnd = null, Guid? parentId = null)
        {
            return await ExecuteApi(async () => {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Schedule, ScheduleFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new ScheduleFilter(size, page, sort, name), source.Token);                
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return Ok(result.Data);
            }, "ScheduleApiController", "Get");
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetItem([FromRoute]Guid id)
        {
            return await ExecuteApi(async () => {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Schedule, ScheduleFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(id, source.Token);
                if (result.UserId != userId) return BadRequest("Found schedule of another user");
                return Ok(result);
            }, "ScheduleApiController", "GetItem");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody]ScheduleCreator creator)
        {
            return await ExecuteApi(async () => {
                var userId = Guid.Parse(User.Identity.Name);
                var selectService = _serviceProvider.GetRequiredService<IProjectSelectService>();
                var userSettingsRepo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.UserSettings>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var userSettings = (await userSettingsRepo.GetAsync(new DB.Context.Filter<DB.Context.UserSettings>()
                {
                    Selector = s => s.UserId == userId
                }, source.Token)).Data.Single();

                var result = await selectService.AddProjectToSchedule(userId, userSettings, creator.ProjectId, creator.BeginDate, creator.SetBeginDate);
                return Ok(result);
            }, "ScheduleApiController", "Create");
        }
    }
}
