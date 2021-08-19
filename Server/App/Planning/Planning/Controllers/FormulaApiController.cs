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
    [Route("api/v1/formulas")]
    [Produces("application/json")]
    [ApiController]
    public class FormulaApiController : CommonControllerBase
    {
        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        public FormulaApiController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<FormulaApiController>>();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(string name = null, int size = 10, 
            int page = 0, string sort = null)
        {
            try
            {               
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Formula, FormulaFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(new FormulaFilter(size, page, sort, name, null), source.Token);                
                Response?.Headers?.Add("x-pages", result.PageCount.ToString());
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка в методе Get контроллера FormulaApiController: {ex.Message} {ex.StackTrace}");
                return InternalServerError($"Ошибка в методе Get контроллера FormulaApiController: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetItem([FromRoute]Guid id)
        {
            try
            {               
                var _dataService = _serviceProvider.GetRequiredService<IGetDataService<Formula, FormulaFilter>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.GetAsync(id, source.Token);               
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка в методе GetItem контроллера FormulaApiController: {ex.Message} {ex.StackTrace}");
                return InternalServerError($"Ошибка в методе GetItem контроллера FormulaApiController: {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] FormulaUpdater updater)
        {
            try
            {               
                var _dataService = _serviceProvider.GetRequiredService<IUpdateDataService<Formula, FormulaUpdater>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.UpdateAsync(updater, source.Token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка в методе Update контроллера FormulaApiController: {ex.Message} {ex.StackTrace}");
                return InternalServerError($"Ошибка в методе Update контроллера FormulaApiController: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] FormulaCreator creator)
        {
            try
            {
                var _dataService = _serviceProvider.GetRequiredService<IAddDataService<Formula, FormulaCreator>>();
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _dataService.AddAsync(creator, source.Token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка в методе Create контроллера FormulaApiController: {ex.Message} {ex.StackTrace}");
                return InternalServerError($"Ошибка в методе Create контроллера FormulaApiController: {ex.Message}");
            }
        }
    }
}
