using Microsoft.Extensions.DependencyInjection;
using Planning.DB.Context;
using Planning.DB.Repository;
using Planning.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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

    public class RepositoryTest : IClassFixture<CustomFixture>
    {
        private readonly IServiceProvider _serviceProvider;
        private CustomFixture _fixture;

        public RepositoryTest(CustomFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
            _fixture = fixture;            
        }

        ///// <summary>
        ///// Тест получения списка сущностей по фильтру
        ///// </summary>
        ///// <returns></returns>
        //[Fact]
        //public async Task GetTest()
        //{
        //    var context = _serviceProvider.GetRequiredService<DbPgContext>();
        //    AddUsers(context, "user_select_{0}", "user_description_{0}", "user_login_{0}", "user_password_{0}", 10);
        //    AddUsers(context, "user_not_select_{0}", "user_description_{0}", "user_login_{0}", "user_password_{0}", 10);
        //    await context.SaveChangesAsync();

        //    var repo = _serviceProvider.GetRequiredService<IRepository<User>>();
        //    var data = await repo.GetAsync(new Filter<User>()
        //    {
        //        Page = 0,
        //        Size = 10,
        //        Selector = s => s.Name.Contains("user_select")
        //    }, CancellationToken.None);

        //    Assert.Equal(10, data.Data.Count());
        //    foreach (var item in data.Data)
        //    {
        //        Assert.Contains("user_select", item.Name);
        //    }
        //}

        ///// <summary>
        ///// Тест получения сущности по id
        ///// </summary>
        ///// <returns></returns>
        //[Fact]
        //public async Task GetItemTest()
        //{
        //    var context = _serviceProvider.GetRequiredService<DbPgContext>();
        //    var user = CreateUser("user_{0}", "user_description_{0}", "user_login_{0}", "user_password_{0}");
        //    context.Users.Add(user);
        //    await context.SaveChangesAsync();

        //    var repo = _serviceProvider.GetRequiredService<IRepository<User>>();
        //    var data = await repo.GetAsync(user.Id, CancellationToken.None);

        //    Assert.NotNull(data);
        //    Assert.Equal(user.Id, data.Id);
        //}

        /// <summary>
        /// Тест добавления сущности
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddTest()
        {
            var context = _serviceProvider.GetRequiredService<DbPgContext>();
            var repo = _serviceProvider.GetRequiredService<IRepository<User>>();
            var formulaRepo = _serviceProvider.GetRequiredService<IRepository<Formula>>();
            var formula = _fixture.CreateFormula();
            await formulaRepo.AddAsync(formula, true, CancellationToken.None);

            var user = _fixture.CreateUser("user_{0}", "user_description_{0}", "user_login_{0}", "user_password_{0}", formula.Id);

            var result = await repo.AddAsync(user, true, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(user.Name, result.Name);

            var actual = context.Users.FirstOrDefault(s=>s.Id == result.Id);
            Assert.NotNull(actual);
            Assert.Equal(user.Name, actual.Name);
        }

        ///// <summary>
        ///// Тест удаления сущности
        ///// </summary>
        ///// <returns></returns>
        //[Fact]
        //public async Task DeleteTest()
        //{
        //    var context = _serviceProvider.GetRequiredService<DbPgContext>();
        //    var repo = _serviceProvider.GetRequiredService<IRepository<User>>();
        //    var user = CreateUser("user_{0}", "user_description_{0}", "user_login_{0}", "user_password_{0}");

        //    context.Users.Add(user);
        //    await context.SaveChangesAsync();

        //    var data = await repo.DeleteAsync(user, true, CancellationToken.None);

        //    Assert.NotNull(data);
        //    Assert.Equal(user.Id, data.Id);
        //    Assert.True(data.IsDeleted);

        //    var test = context.Users.FirstOrDefault(s => s.Id == user.Id);
        //    Assert.NotNull(test);
        //    Assert.Equal(user.Id, test.Id);
        //    Assert.True(test.IsDeleted);
        //}



        private void AddUsers(DbPgContext context, string nameMask, string descriptionMask, string loginMask, string passwordMask, int count, Guid formulaId)
        {
            for (int i = 0; i < count; i++)
            {
                var user = _fixture.CreateUser(nameMask, descriptionMask, loginMask, passwordMask, formulaId);
                context.Users.Add(user);
            }
        }

       
    }
}
