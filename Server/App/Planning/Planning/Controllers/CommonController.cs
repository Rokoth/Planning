//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Deploy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.Common;
using System;
using System.Threading.Tasks;

namespace Planning.Controllers
{
    /// <summary>
    /// Контроллер общих методов (без првязке к к-либо модели)
    /// </summary>
    [Route("api/v1/common")]
    [Produces("application/json")]
    public class CommonController : CommonControllerBase
    {
        private IDeployService _deployService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceProvider"></param>
        public CommonController(ILogger<CommonController> logger, IDeployService deployService) : base(logger)
        {
            _deployService = deployService;
        }

        /// <summary>
        /// Проверка доступности сервиса
        /// </summary>
        /// <returns></returns>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok();
        }

        /// <summary>
        /// Выполнить деплой БД (подготовленных запросов)
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("deploy")]
        public async Task<IActionResult> Deploy()
        {
            return await ExecuteApi(async () =>
            {               
                await _deployService.Deploy();
                return Ok();
            }, "CommonController", "Deploy");
        }
    }
}
