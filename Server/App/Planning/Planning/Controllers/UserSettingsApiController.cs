using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.Contracts.Model;
using Planning.Service;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Controllers
{
    [Route("api/v1/users")]
    [Produces("application/json")]
    [ApiController]
    public class UserSettingsApiController : CommonControllerBase
    {
        private IGetDataService<UserSettings, UserSettingsFilter> _dataService;
        private IUpdateDataService<UserSettings, UserSettingsUpdater> _updateDataService;
        private IAddDataService<UserSettings, UserSettingsCreator> _addDataService;
        private IDeleteDataService<UserSettings> _deleteDataService;

        public UserSettingsApiController(ILogger<UserSettingsApiController> logger,
             IGetDataService<UserSettings, UserSettingsFilter> dataService,
            IUpdateDataService<UserSettings, UserSettingsUpdater> updateDataService,
            IAddDataService<UserSettings, UserSettingsCreator> addDataService,            
            IDeleteDataService<UserSettings> deleteDataService) : base(logger)
        {
            _dataService = dataService;
            _updateDataService = updateDataService;
            _addDataService = addDataService;           
            _deleteDataService = deleteDataService;
        }

        [HttpGet]
        [Authorize("Token")]
        public async Task<IActionResult> Get(Guid id, int size = 10,
            int page = 0, string sort = null)
        {
            return await ExecuteApi(async () => {
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new UserSettingsFilter(size, page, sort, id), source.Token);
                Response?.Headers?.Add("x-pages", result.PageCount.ToString());
                return Ok(result.Data);
            }, "UserSettingsApiController", "Get");
        }

        [HttpGet("{id}")]
        [Authorize("Token")]
        public async Task<IActionResult> GetItem([FromRoute] Guid id)
        {
            return await ExecuteApi(async () => {
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(id, source.Token);
                return Ok(result);
            }, "UserSettingsApiController", "GetItem");
        }

        [HttpPut]
        [Authorize("Token")]
        public async Task<IActionResult> Update([FromBody] UserSettingsUpdater updater)
        {
            return await ExecuteApi(async () => {
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _updateDataService.UpdateAsync(updater, source.Token);
                return Ok(result);
            }, "UserSettingsApiController", "Update");
        }

        [HttpPost]
        [Authorize("Token")]
        public async Task<IActionResult> Create([FromBody] UserSettingsCreator creator)
        {
            return await ExecuteApi(async () => {
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _addDataService.AddAsync(creator, source.Token);
                return Ok(result);
            }, "UserSettingsApiController", "Create");
        }

        //[HttpGet("history")]
        //[Authorize("Token")]
        //public async Task<IActionResult> GetHistory([FromRoute] Guid id, [FromQuery] int page = 0, [FromQuery] int size = 10,
        //    [FromQuery] string sort = null, [FromQuery] string name = null)
        //{
        //    return await ExecuteApi(async () => {
        //        var _dataService = _serviceProvider.GetRequiredService<IGetDataService<UserSettingsHistory, UserSettingsHistoryFilter>>();
        //        CancellationTokenSource source = new CancellationTokenSource(30000);
        //        var result = await _dataService.GetAsync(new UserSettingsHistoryFilter(id, size, page, sort, name), source.Token);
        //        Response.Headers.Add("x-pages", result.PageCount.ToString());
        //        return Ok(result.Data);
        //    }, "UserSettingsApiController", "GetHistory");
        //}
    }
}
