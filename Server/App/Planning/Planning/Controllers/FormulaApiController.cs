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
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Controllers
{
    [Route("api/v1/formulas")]
    [Produces("application/json")]
    [ApiController]
    public class FormulaApiController : CommonControllerBase
    {
        private IGetDataService<Formula, FormulaFilter> _dataService;
        private IUpdateDataService<Formula, FormulaUpdater> _updateDataService;
        private IAddDataService<Formula, FormulaCreator> _addDataService;
        private IGetDataService<FormulaHistory, FormulaHistoryFilter> _historyDataService;

        public FormulaApiController(ILogger<FormulaApiController> logger, 
            IGetDataService<Formula, FormulaFilter> dataService,
            IUpdateDataService<Formula, FormulaUpdater> updateDataService,
            IAddDataService<Formula, FormulaCreator> addDataService,
            IGetDataService<FormulaHistory, FormulaHistoryFilter> historyDataService
            ) : base(logger)
        {
            _dataService = dataService;
            _updateDataService = updateDataService;
            _addDataService = addDataService;
            _historyDataService = historyDataService;
        }

        [HttpGet]
        [Authorize("Token")]
        public async Task<IActionResult> Get(string name = null, int size = 10,
            int page = 0, string sort = null)
        {
            return await ExecuteApi(async () => {                
                CancellationTokenSource source = new(30000);
                var result = await _dataService.GetAsync(new FormulaFilter(size, page, sort, name, null), source.Token);
                Response?.Headers?.Add("x-pages", result.PageCount.ToString());
                return Ok(result.Data);
            }, "FormulaApiController", "Get");
        }

        [HttpGet("{id}")]
        [Authorize("Token")]
        public async Task<IActionResult> GetItem([FromRoute] Guid id)
        {
            return await ExecuteApi(async () => {               
                CancellationTokenSource source = new(30000);
                var result = await _dataService.GetAsync(id, source.Token);
                return Ok(result);
            }, "FormulaApiController", "GetItem");
        }

        [HttpPut]
        [Authorize("Token")]
        public async Task<IActionResult> Update([FromBody] FormulaUpdater updater)
        {
            return await ExecuteApi(async () => {               
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _updateDataService.UpdateAsync(updater, source.Token);
                return Ok(result);
            }, "FormulaApiController", "Update");
        }

        [HttpPost]
        [Authorize("Token")]
        public async Task<IActionResult> Create([FromBody] FormulaCreator creator)
        {
            return await ExecuteApi(async () => {                
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _addDataService.AddAsync(creator, source.Token);
                return Ok(result);
            }, "FormulaApiController", "Create");
        }

        [HttpGet("history")]
        [Authorize("Token")]
        public async Task<IActionResult> GetHistory([FromRoute] Guid id, [FromQuery] int page = 0, [FromQuery] int size = 10,
            [FromQuery] string sort = null, [FromQuery] string name = null)
        {
            return await ExecuteApi(async () => {
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var result = await _historyDataService.GetAsync(new FormulaHistoryFilter(id, size, page, sort, name), source.Token);
                Response.Headers.Add("x-pages", result.PageCount.ToString());
                return Ok(result.Data);
            }, "FormulaApiController", "GetHistory");
        }
    }
}
