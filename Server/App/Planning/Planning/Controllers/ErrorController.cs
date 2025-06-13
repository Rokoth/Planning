using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Planning.Contract.Model;

namespace Planning.Controllers
{
    /// <summary>
    /// Controller for error responses
    /// </summary>
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="message"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public IActionResult Index([FromQuery] string message, [FromQuery] string source = null)
        {
            return View(new ErrorMessage()
            {
                Message = message,
                Source = source
            });
        }
    }
}