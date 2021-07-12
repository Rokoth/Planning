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
    [Route("api/v1/projects")]
    [Produces("application/json")]
    [ApiController]
    public class ProjectApiController : CommonControllerBase
    {
        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        public ProjectApiController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<ProjectApiController>>();
        }

        [HttpGet]
        public async Task<IActionResult> Get(string name = null, int size = 10, 
            int page = 0, string sort = null, bool? isLeaf = null, DateTimeOffset? lastUsedDateBegin = null, DateTimeOffset? lastUsedDateEnd = null, Guid? parentId = null)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new ProjectFilter(userId, size, page, sort, name, isLeaf,
                    lastUsedDateBegin, lastUsedDateEnd, parentId), source.Token);                
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка в методе Get контроллера ProjectApiController: {ex.Message} {ex.StackTrace}");
                return InternalServerError($"Ошибка в методе Get контроллера ProjectApiController: {ex.Message}");
            }
        }
    }
}
