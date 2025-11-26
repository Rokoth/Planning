//Copyright 2025 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref 1
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Planning.Contracts.Model;
using Planning.Service;
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

        #region Private fields
        private readonly IAuthService _authService; 
        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="authService"></param>
        public AccountController(ILogger<AccountController> logger, IAuthService authService) : base(logger)
        {
            _authService = authService;
        }

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
        public Task<IActionResult> Login(UserIdentity userIdentity, string returnUrl)
            => Execute(() => LoginInternal(userIdentity, returnUrl), nameof(AccountController), nameof(Login));

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

        private async Task<IActionResult> LoginInternal(UserIdentity userIdentity, string returnUrl)
        {
            if (!ModelState.IsValid)
                return base.RedirectToAction(nameof(Login));

            var identity = await GetIdentity(userIdentity);
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

        private Task<ClaimsIdentity> GetIdentity(UserIdentity userIdentity)
            => _authService.Auth(userIdentity, GetToken());

        private static CancellationToken GetToken()
            => new CancellationTokenSource(CancellationTokenSourceDelay).Token;

        private async Task<IActionResult> LogoutInternal()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(IndexMethodName, HomeControllerName);
        } 

        #endregion
    }
}
