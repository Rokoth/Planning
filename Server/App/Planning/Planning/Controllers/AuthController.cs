using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Planning.Common;
using Planning.Contract.Model;
using Planning.Service;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IServiceProvider _serviceProvider;


        public AuthController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }

        [HttpPost("auth")]
        public async Task<IActionResult> Auth([FromBody] Contract.Model.UserIdentity login)
        {
            try
            {
                var source = new CancellationTokenSource(30000);
                var dataService = _serviceProvider.GetRequiredService<IAuthService>();

                var identity = await dataService.AuthApi(login, source.Token);
                if (identity == null)
                {
                    return BadRequest(new { errorText = "Invalid username or password." });
                }

                var now = DateTime.UtcNow;
                // создаем JWT-токен
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
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }
    }
}
