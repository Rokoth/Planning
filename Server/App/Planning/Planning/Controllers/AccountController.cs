using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Planning.Contract.Model;
using Planning.Service;

namespace Planning.Controllers
{



    /// <summary>
    /// Authentification methods
    /// </summary>
    public class AccountController : CommonControllerBase
    {
        public AccountController(IServiceProvider serviceProvider): base(serviceProvider)
        {
            
        }
        
        // GET: AccountController/Login
        /// <summary>
        /// Login page
        /// </summary>
        /// <returns></returns>
        public ActionResult Login(string returnUrl)
        {            
            return View();
        }

        // POST: AccountController/Login
        /// <summary>
        /// Login method
        /// </summary>
        /// <param name="userIdentity">login and password</param>
        /// <param name="returnUrl">url to redirect after authorization</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserIdentity userIdentity, string returnUrl)
        {
            return await Execute(async ()=> {
                if (ModelState.IsValid)
                {
                    var source = new CancellationTokenSource(30000);
                    var dataService = _serviceProvider.GetRequiredService<IAuthService>();
                    var identity = await dataService.Auth(userIdentity, source.Token);
                    if (identity == null)
                    {
                        return RedirectToAction("Index", "Error", new { Message = "Неверный логин или пароль" });
                    }
                    // установка аутентификационных куки
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                }
                else
                {
                    return RedirectToAction("Login");
                }
                if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }, "AccountController", "Login");
        }

        // POST: AccountController/Logout
        /// <summary>
        /// LogOut method
        /// </summary>
        /// <returns></returns>
        [HttpGet]       
        public async Task<IActionResult> Logout()
        {
            return await Execute(async () =>
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }, "AccountController", "Logout");
        }
    }
}
