using Deploy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.Common;
using Planning.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Controllers
{
    [Route("api/v1/common")]
    [Produces("application/json")]
    public class CommonController : CommonControllerBase
    {
              

        public CommonController(IServiceProvider serviceProvider): base(serviceProvider)
        {            
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok();
        }

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

        [HttpPost("send_error")]
        [Authorize]
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
