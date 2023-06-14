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
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceProvider"></param>
        public CommonController(IServiceProvider serviceProvider): base(serviceProvider)
        {            
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
                var deployService = _serviceProvider.GetRequiredService<IDeployService>();
                await deployService.Deploy();
                return Ok();
            }, "CommonController", "Deploy");
        }

        /// <summary>
        /// Отправить сообщение об ошибке или оставить отзыв (сервис TaskCollector)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("send_error")]        
        public async Task<IActionResult> SendErrorMessage([FromBody] ErrorNotifyMessage message)
        {
            try
            {
                await errorNotifyService.Send(message.Message, message.MessageLevel, message.Title);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at SendErrorMessage: {ex.Message} {ex.StackTrace}");
                return InternalServerError(ex.Message);
            }
        }
    }
}
