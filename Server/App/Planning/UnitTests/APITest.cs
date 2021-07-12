using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Planning.Controllers;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Planning.UnitTests
{
    /// <summary>
    /// api unit tests
    /// </summary>
    public class APITest : IClassFixture<CustomFixture>
    {
        private readonly IServiceProvider _serviceProvider;

        public APITest(CustomFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        /// <summary>
        /// AuthController. Test for Auth method (positive scenario)
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ApiAuthTest()
        {
            var formula = await AddFormula();
            var user = await AddUser(formula.Id);
            
            await AuthAndAssert(user);
        }

        
        private async Task AuthAndAssert(DB.Context.User user)
        {
            var clientController = new AuthController(_serviceProvider);
            var result = await clientController.Auth(new Contract.Model.UserIdentity()
            {
                Login = user.Login,
                Password = $"user_password_{user.Id}"
            });
            var response = result as OkObjectResult;
            Assert.NotNull(response);
            JObject value = JObject.FromObject(response.Value);
            Assert.Equal(user.Id.ToString(), value["UserName"].ToString());
        }
                
        private async Task<DB.Context.User> AddUser(Guid formulaId)
        {
            var context = _serviceProvider.GetRequiredService<DB.Context.DbPgContext>();
            var user = CreateUser(formulaId);
            context.Set<DB.Context.User>().Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        private async Task<DB.Context.Formula> AddFormula()
        {
            var context = _serviceProvider.GetRequiredService<DB.Context.DbPgContext>();
            var formula = CreateFormula();
            context.Set<DB.Context.Formula>().Add(formula);
            await context.SaveChangesAsync();
            return formula;
        }

        private DB.Context.User CreateUser(Guid formulaId)
        {
            var user_id = Guid.NewGuid();
            return new DB.Context.User()
            {
                Name = $"user_{user_id}",
                Id = user_id,
                Description = $"user_description_{user_id}",
                IsDeleted = false,
                Login = $"user_login_{user_id}",
                Password = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes($"user_password_{user_id}")),
                VersionDate = DateTimeOffset.Now,
                FormulaId = formulaId
            };
        }

        private DB.Context.Formula CreateFormula ()
        {
            var formula_id = Guid.NewGuid();
            return new DB.Context.Formula()
            {
                Name = $"formula_{formula_id}",
                Id = formula_id,
                IsDeleted = false,
                IsDefault = true,
                Text = "Min(SelectCount)",
                VersionDate = DateTimeOffset.Now
            };
        }
    }
}
