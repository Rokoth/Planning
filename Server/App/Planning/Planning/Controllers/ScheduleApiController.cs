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
        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        public ScheduleApiController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<ScheduleApiController>>();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(string name = null, string path = null, int size = 10, 
            int page = 0, string sort = null, bool? isLeaf = null, DateTimeOffset? lastUsedDateBegin = null, DateTimeOffset? lastUsedDateEnd = null, Guid? parentId = null)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Schedule, ScheduleFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new ScheduleFilter(size, page, sort, name), source.Token);                
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка в методе Get контроллера ScheduleApiController: {ex.Message} {ex.StackTrace}");
                return InternalServerError($"Ошибка в методе Get контроллера ScheduleApiController: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetItem([FromRoute]Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Schedule, ScheduleFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(id, source.Token);
                if (result.UserId != userId) return BadRequest("Found schedule of another user");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка в методе GetItem контроллера ScheduleApiController: {ex.Message} {ex.StackTrace}");
                return InternalServerError($"Ошибка в методе GetItem контроллера ScheduleApiController: {ex.Message}");
            }
        }
    }
}
