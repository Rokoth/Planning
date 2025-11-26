using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.Common;
using Planning.Contracts.Model;
using Planning.Models;
using Planning.Service;

namespace Planning.Controllers
{
    public class HomeController :  CommonControllerBase
    {
        public HomeController(ILogger<FormulaController> logger) : base(logger)
        {            
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [Authorize]
        public IActionResult Deploy()
        {
            return View();
        }

        [Authorize]
        public IActionResult SendReport()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
