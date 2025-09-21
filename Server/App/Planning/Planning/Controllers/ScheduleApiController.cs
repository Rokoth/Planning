using Contracts.Model.Schedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Route("api/v1/shedules")]
    [Produces("application/json")]
    [ApiController]
    public class ScheduleApiController : CommonControllerBase
    {
        private IGetDataService<Schedule, ScheduleFilter> _dataService;
        private IProjectSelectService _projectSelectService;
        private DB.Repository.IRepository<DB.Context.UserSettings> _userSettingsRepo;

        public ScheduleApiController(
            IGetDataService<Schedule, ScheduleFilter> dataService,
            IProjectSelectService projectSelectService,
            DB.Repository.IRepository<DB.Context.UserSettings> userSettingsRepo,
            ILogger<ScheduleApiController> logger) : base(logger)
        {
            _dataService = dataService;
            _projectSelectService = projectSelectService;
            _userSettingsRepo = userSettingsRepo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(string name = null, string path = null, int size = 10, 
            int page = 0, string sort = null, bool? onlyActive = null, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null)
        {
            return await ExecuteApi(async () => {
                var userId = Guid.Parse(User.Identity.Name);                
                CancellationTokenSource source = new(30000);
                var result = await _dataService.GetAsync(new ScheduleFilter(size, page, sort, name, null, userId, onlyActive, fromDate, toDate), source.Token);                
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
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var userSettings = (await _userSettingsRepo.GetAsync(new DB.Context.Filter<DB.Context.UserSettings>()
                {
                    Selector = s => s.UserId == userId
                }, source.Token)).Data.Single();

                var result = await _projectSelectService.MoveToNextSchedule(userId, creator.ProjectId, creator.DirectionId, creator.BeginDate, creator.SetBeginDate);
                return Ok(result);
            }, "ScheduleApiController", "Create");
        }
    }
}
