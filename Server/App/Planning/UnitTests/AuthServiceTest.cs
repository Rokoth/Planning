using Microsoft.Extensions.DependencyInjection;
using Planning.DB.Context;
using Planning.Service;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Planning.UnitTests
{
    public class AuthServiceTest : IClassFixture<CustomFixture>
    {
        private readonly IServiceProvider _serviceProvider;
        private CustomFixture _fixture;

        public AuthServiceTest(CustomFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
            _fixture = fixture;
        }

        [Fact]
        public async Task AuthTest()
        {
            var context = _serviceProvider.GetRequiredService<DbPgContext>();
            var formula = _fixture.CreateFormula();
            var user = _fixture.CreateUser("user_{0}", "user_description_{0}", "user_login_{0}", "user_password_{0}", formula.Id);
            context.Formulas.Add(formula);
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var authService = _serviceProvider.GetRequiredService<IAuthService>();
            var result = await authService.Auth(new Contract.Model.UserIdentity() {
               Login = user.Login,
               Password = $"user_password_{user.Id}"
            }, CancellationToken.None);
            Assert.NotNull(result);
            Assert.True(result.IsAuthenticated);
            var check = result.Claims.FirstOrDefault(s => s.Type == ClaimsIdentity.DefaultNameClaimType);
            Assert.Equal(user.Id.ToString(), check.Value);

            var result2 = await authService.AuthApi(new Contract.Model.UserIdentity()
            {
                Login = user.Login,
                Password = $"user_password_{user.Id}"
            }, CancellationToken.None);
            Assert.NotNull(result2);
            Assert.True(result2.IsAuthenticated);
            var check2 = result2.Claims.FirstOrDefault(s => s.Type == ClaimsIdentity.DefaultNameClaimType);
            Assert.Equal(user.Id.ToString(), check2.Value);
        }
    }
}
