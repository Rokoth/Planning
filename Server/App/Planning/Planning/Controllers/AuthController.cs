//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Planning.Common;
using Planning.Contract.Model;
using Planning.Service;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Controllers
{
    /// <summary>
    /// Auth api controller
    /// </summary>
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : CommonControllerBase
    {
        public AuthController(IServiceProvider serviceProvider): base(serviceProvider)
        {
            
        }

        /// <summary>
        /// Auth method
        /// </summary>
        /// <param name="login">login</param>
        /// <returns></returns>
        [HttpPost("auth")]
        public async Task<IActionResult> Auth([FromBody] UserIdentity login)
        {
            return await ExecuteApi(async ()=> {
                var source = new CancellationTokenSource(30000);
                var dataService = _serviceProvider.GetRequiredService<IAuthService>();

                var identity = await dataService.AuthApi(login, source.Token);
                if (identity == null)
                {
                    return BadRequest(new { errorText = "Invalid username or password." });
                }

                var now = DateTime.UtcNow;
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                        claims: identity.Claims,
                        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new ClientIdentityResponse
                {
                    Token = encodedJwt,
                    UserName = identity.Name
                };

                return Ok(response);
            }, "AuthController", "Auth");
        }
    }
}
