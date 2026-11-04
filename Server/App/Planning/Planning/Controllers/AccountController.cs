using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Planning.Service;
using Contracts.Model.User;
using Microsoft.Extensions.Logging;

namespace Planning.Controllers
{
    /// <summary>
    /// Authentification methods
    /// </summary>
    public class AccountController : CommonControllerBase 
    {
        #region Constants
        private const string WrongIdentityMessage = "Неверный логин или пароль";
        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="authService"></param>
        public AccountController(ILogger<AccountController> logger) : base(logger) {}

        #region Public

        // GET: AccountController/Login
        /// <summary>
        /// Login page
        /// </summary>
        /// <param name="returnUrl">url to redirect after authorization</param>
        /// <returns></returns>
        public ActionResult Login(string returnUrl) => View();

        // POST: AccountController/Login
        /// <summary>
        /// Login method
        /// </summary>
        /// <param name="userIdentity">login and password</param>
        /// <param name="returnUrl">url to redirect after authorization</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Login([FromServices]IAuthService authService, [FromBody]UserIdentity userIdentity, string returnUrl)
            => Execute(() => LoginInternal(authService, userIdentity, returnUrl), nameof(AccountController), nameof(Login));

        // POST: AccountController/Logout
        /// <summary>
        /// LogOut method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<IActionResult> Logout()
            => Execute(LogoutInternal, nameof(AccountController), nameof(Logout));

        #endregion

        #region Private

        private async Task<IActionResult> LoginInternal(IAuthService authService,UserIdentity userIdentity, string returnUrl)
        {
            if (!ModelState.IsValid)
                return base.RedirectToAction(nameof(Login));

            var identity = await GetIdentity(authService, userIdentity);
            if (identity == null)
                return base.RedirectToAction(IndexMethodName, ErrorControllerName, new { Message = WrongIdentityMessage });

            return await ReturnSuccessResult(returnUrl, identity);
        }

        private async Task<IActionResult> ReturnSuccessResult(string returnUrl, ClaimsIdentity identity)
        {
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            if (!string.IsNullOrEmpty(returnUrl))
                return base.Redirect(returnUrl);

            return base.RedirectToAction(IndexMethodName, HomeControllerName);
        }

        private static Task<ClaimsIdentity> GetIdentity(IAuthService authService, UserIdentity userIdentity)
            => authService.Auth(userIdentity, GetToken());

        private async Task<IActionResult> LogoutInternal()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(IndexMethodName, HomeControllerName);
        } 

        #endregion
    }
}
