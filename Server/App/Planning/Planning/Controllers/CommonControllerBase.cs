using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.Common;
using System;
using System.Threading.Tasks;

namespace Planning.Controllers
{
    public abstract class CommonControllerBase : Controller
    {
        protected ILogger _logger;
        protected readonly IErrorNotifyService errorNotifyService;
        protected IServiceProvider _serviceProvider;

        public CommonControllerBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<CommonControllerBase>>();
            errorNotifyService = serviceProvider.GetRequiredService<IErrorNotifyService>();
        }

        protected async Task<IActionResult> Execute(Func<Task<IActionResult>> action, string controller, string method)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at {controller}::{method}: {ex.Message} {ex.StackTrace}");
                await errorNotifyService.Send($"Error at {controller}::{method}: {ex.Message} {ex.StackTrace}");
                return RedirectToAction("Index", "Error", new { Message = $"Error at {controller}::{method}: {ex.Message}" });                
            }
        }

        protected async Task<IActionResult> ExecuteApi(Func<Task<IActionResult>> action, string controller, string method)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at {controller}::{method}: {ex.Message} {ex.StackTrace}");
                await errorNotifyService.Send($"Error at {controller}::{method}: {ex.Message} {ex.StackTrace}");
                return InternalServerError($"Error at {controller}::{method}: {ex.Message}");
            }
        }

        protected InternalServerErrorObjectResult InternalServerError()
        {
            return new InternalServerErrorObjectResult();
        }

        protected InternalServerErrorObjectResult InternalServerError(object value)
        {
            return new InternalServerErrorObjectResult(value);
        }       
    }
}
