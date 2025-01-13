using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Planning.Contract.Model;
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
        public UserSettingsApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        [HttpGet]
        [Authorize("Token")]
        public async Task<IActionResult> Get(Guid id, int size = 10,
            int page = 0, string sort = null)
        {
            return await ExecuteApi(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<UserSettings, UserSettingsFilter>>();
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
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<UserSettings, UserSettingsFilter>>();
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
                var _dataService = _serviceProvider.GetRequiredService<IUpdateDataService<UserSettings, UserSettingsUpdater>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.UpdateAsync(updater, source.Token);
                return Ok(result);
            }, "UserSettingsApiController", "Update");
        }

        [HttpPost]
        [Authorize("Token")]
        public async Task<IActionResult> Create([FromBody] UserSettingsCreator creator)
        {
            return await ExecuteApi(async () => {
                var _dataService = _serviceProvider.GetRequiredService<IAddDataService<UserSettings, UserSettingsCreator>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.AddAsync(creator, source.Token);
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
