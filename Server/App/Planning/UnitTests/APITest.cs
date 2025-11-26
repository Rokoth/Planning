using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Planning.Contracts.Model;
using Planning.Controllers;
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
            var formula = await AddFormula("default_formula_{0}");
            var user = await AddUser(formula.Id);
            
            await AuthAndAssert(user);
        }

        /// <summary>
        /// FormulaController. Test for Update method (positive scenario)
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FormulaUpdateTest()
        {
            var formula = await AddFormula("default_formula_{0}");
            var user = await AddUser(formula.Id);
            var identity = await AuthAndAssert(user);

            var testFormula = await AddFormula("formula_{0}");
            var newName = testFormula.Name + "_changed";
            FormulaApiController controller = new FormulaApiController(
                _serviceProvider.GetRequiredService<ILogger<FormulaApiController>>(),
                _serviceProvider.GetRequiredService<IGetDataService<Formula, FormulaFilter>>(),
                _serviceProvider.GetRequiredService<IUpdateDataService<Formula, FormulaUpdater>>(),
                _serviceProvider.GetRequiredService<IAddDataService<Formula, FormulaCreator>>(),
                _serviceProvider.GetRequiredService<IGetDataService<FormulaHistory, FormulaHistoryFilter>>()
                );            
            var res = await controller.Update(new FormulaUpdater()
            { 
               Id = testFormula.Id,
               Name = newName,
               Text = testFormula.Text
            });
            Assert.True(res is OkObjectResult);
            var result = res as OkObjectResult;
            var changed = JObject.FromObject(result.Value).ToObject<Formula>();
            Assert.Equal(newName, changed.Name);

            var context = _serviceProvider.GetRequiredService<DB.Context.DbPgContext>();
            var actual = context.Formulas.Where(s => s.Id == testFormula.Id).FirstOrDefault();
            Assert.Equal(newName, actual.Name);
        }

        /// <summary>
        /// FormulaController. Test for Add method (positive scenario)
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FormulaAddTest()
        {
            var formula = await AddFormula("default_formula_{0}");
            var user = await AddUser(formula.Id);
            var identity = await AuthAndAssert(user);

            var testName = $"formula_{Guid.NewGuid()}";
            FormulaApiController controller = new FormulaApiController(_serviceProvider.GetRequiredService<ILogger<FormulaApiController>>(),
                _serviceProvider.GetRequiredService<IGetDataService<Formula, FormulaFilter>>(),
                _serviceProvider.GetRequiredService<IUpdateDataService<Formula, FormulaUpdater>>(),
                _serviceProvider.GetRequiredService<IAddDataService<Formula, FormulaCreator>>(),
                _serviceProvider.GetRequiredService<IGetDataService<FormulaHistory, FormulaHistoryFilter>>());
            var res = await controller.Create(new FormulaCreator()
            {               
                Name = testName,
                Text = ""
            });
            Assert.True(res is OkObjectResult);
            var result = res as OkObjectResult;
            var changed = JObject.FromObject(result.Value).ToObject<Formula>();
            Assert.Equal(testName, changed.Name);

            var context = _serviceProvider.GetRequiredService<DB.Context.DbPgContext>();
            var actual = context.Formulas.Where(s => s.Id == changed.Id).FirstOrDefault();
            Assert.Equal(testName, actual.Name);
        }

        /// <summary>
        /// FormulaController. Test for Get method (positive scenario)
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FormulaGetTest()
        {
            var formula = await AddFormula("default_formula_{0}");
            var user = await AddUser(formula.Id);
            var identity = await AuthAndAssert(user);

            await AddFormulas("formula_select_{0}", 10);
            await AddFormulas("formula_not_select_{0}", 10);
            FormulaApiController controller = new FormulaApiController(_serviceProvider.GetRequiredService<ILogger<FormulaApiController>>(),
                _serviceProvider.GetRequiredService<IGetDataService<Formula, FormulaFilter>>(),
                _serviceProvider.GetRequiredService<IUpdateDataService<Formula, FormulaUpdater>>(),
                _serviceProvider.GetRequiredService<IAddDataService<Formula, FormulaCreator>>(),
                _serviceProvider.GetRequiredService<IGetDataService<FormulaHistory, FormulaHistoryFilter>>());
            var res = await controller.Get("formula_select", 10, 0, null);
            Assert.True(res is OkObjectResult);
            var result = res as OkObjectResult;
            var actuals = JArray.FromObject(result.Value);
            Assert.Equal(10, actuals.Count);
            foreach (var assert in actuals)
            {
                var actual = assert.ToObject<Formula>();
                Assert.Contains("formula_select", actual.Name);
            }           
        }

        /// <summary>
        /// FormulaController. Test for Get method (positive scenario)
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FormulaGetItemTest()
        {
            var formula = await AddFormula("default_formula_{0}");
            var user = await AddUser(formula.Id);
            var identity = await AuthAndAssert(user);

            var testFormula = await AddFormula("formula_select_{0}");
            
            FormulaApiController controller = new FormulaApiController(_serviceProvider.GetRequiredService<ILogger<FormulaApiController>>(),
                _serviceProvider.GetRequiredService<IGetDataService<Formula, FormulaFilter>>(),
                _serviceProvider.GetRequiredService<IUpdateDataService<Formula, FormulaUpdater>>(),
                _serviceProvider.GetRequiredService<IAddDataService<Formula, FormulaCreator>>(),
                _serviceProvider.GetRequiredService<IGetDataService<FormulaHistory, FormulaHistoryFilter>>());
            var res = await controller.GetItem(testFormula.Id);
            Assert.True(res is OkObjectResult);
            var result = res as OkObjectResult;
            var actual = result.Value as Formula;
            Assert.Equal(testFormula.Id, actual.Id);
        }

        /// <summary>
        /// ProjectController. Test for GetItem method (positive scenario)
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ProjectGetItemTest()
        {
            var formula = await AddFormula("default_formula_{0}");
            var user = await AddUser(formula.Id);
            var identity = await AuthAndAssert(user);

            var testProject = await AddProject("project_select_{0}", user.Id);

            ProjectApiController controller = new(_serviceProvider.GetRequiredService<ILogger<ProjectApiController>>(),
                _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>(),
                _serviceProvider.GetRequiredService<IUpdateDataService<Project, ProjectUpdater>>(),
                _serviceProvider.GetRequiredService<IAddDataService<Project, ProjectCreator>>(),
                _serviceProvider.GetRequiredService<IGetDataService<ProjectHistory, ProjectHistoryFilter>>());
            var res = await controller.GetItem(testProject.Id);

            Assert.True(res is OkObjectResult);
            var result = res as OkObjectResult;
            var actual = result.Value as Project;
            Assert.Equal(testProject.Id, actual.Id);
        }

        /// <summary>
        /// ScheduleController. Test for Add method (positive scenario)
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ScheduleAddTest()
        {
            var formula = await AddFormula("default_formula_{0}");
            var user = await AddUser(formula.Id);
            var identity = await AuthAndAssert(user);

            var testProject = await AddProject("project_select_{0}", user.Id);
          
            ScheduleApiController controller = new(_serviceProvider.GetRequiredService<ILogger<ScheduleApiController>>(),
                _serviceProvider.GetRequiredService<IGetDataService<Schedule, ScheduleFilter>>(),
                _serviceProvider.GetRequiredService<IUpdateDataService<Schedule, ScheduleUpdater>>(),
                _serviceProvider.GetRequiredService<IAddDataService<Schedule, ScheduleCreator>>(),
                _serviceProvider.GetRequiredService<IGetDataService<ScheduleHistory, ScheduleHistoryFilter>>(),
                _serviceProvider.GetRequiredService<IProjectSelectService>(),
                _serviceProvider.GetRequiredService<DB.Repository.IRepository<DB.Context.UserSettings>>());

            var claims = new ClaimsPrincipal(new ClaimsIdentity([
                                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                        new Claim(ClaimTypes.Name, user.Id.ToString())
                                   ], "TestAuthentication"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claims }
            };

            var res = await controller.Create(new ScheduleCreator()
            {
                BeginDate = DateTime.Now,
                ProjectId = testProject.Id,
                SetBeginDate = true,
                UserId = user.Id,
            });

            Assert.True(res is OkObjectResult);
            var result = res as OkObjectResult;
            var actual = result.Value as Schedule;
            Assert.NotNull(actual);

            var context = _serviceProvider.GetRequiredService<DB.Context.DbPgContext>();
            var dbEntry = await context.Set<DB.Context.Schedule>().FirstOrDefaultAsync(s => s.Id == actual.Id);
            Assert.NotNull(dbEntry);
        }

        /// <summary>
        /// ProjectController. Test for Get method (positive scenario)
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ProjectGetTest()
        {



            ProjectApiController controller = new ProjectApiController(_serviceProvider.GetRequiredService<ILogger<ProjectApiController>>(),
                _serviceProvider.GetRequiredService<IGetDataService<Project, ProjectFilter>>(),
                _serviceProvider.GetRequiredService<IUpdateDataService<Project, ProjectUpdater>>(),
                _serviceProvider.GetRequiredService<IAddDataService<Project, ProjectCreator>>(),
                _serviceProvider.GetRequiredService<IGetDataService<ProjectHistory, ProjectHistoryFilter>>());
            var formula = await AddFormula("default_formula_{0}");
            var user = await AddUser(formula.Id);
            var identity = await AuthAndAssert(user);

            await AddProjects("project_select_{0}", user.Id, 10);
            await AddProjects("project_not_select_{0}", user.Id, 10);

            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                        new Claim(ClaimTypes.Name, user.Id.ToString())
                                   }, "TestAuthentication"));

            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = claims };

            var res = await controller.Get("project_select", size: 10, page: 0);
            Assert.True(res is OkObjectResult);
            var result = res as OkObjectResult;
            var actuals = JArray.FromObject(result.Value);
            Assert.Equal(10, actuals.Count);
            foreach (var assert in actuals)
            {
                var actual = assert.ToObject<Contracts.Model.Project>();
                Assert.Contains("project_select", actual.Name);
            }
        }

        private async Task<ClientIdentityResponse> AuthAndAssert(DB.Context.User user)
        {
            var clientController = new AuthController(_serviceProvider.GetRequiredService<ILogger<AuthController>>(),
                _serviceProvider.GetRequiredService<IAuthService>());
            var result = await clientController.Auth(new Contracts.Model.UserIdentity()
            {
                Login = user.Login,
                Password = $"user_password_{user.Id}"
            });
            var response = result as OkObjectResult;
            Assert.NotNull(response);
            var value = JObject.FromObject(response.Value).ToObject<ClientIdentityResponse>();
            Assert.Equal(user.Id.ToString(), value.UserName);
            return value;
        }
                
        private async Task<DB.Context.User> AddUser(Guid formulaId)
        {
            var context = _serviceProvider.GetRequiredService<DB.Context.DbPgContext>();
            var user = CreateUser(formulaId);
            context.Set<DB.Context.User>().Add(user);
            await context.SaveChangesAsync();
            var settings = new DB.Context.UserSettings()
            {
                DefaultProjectTimespan = 3,
                IsDeleted = false,
                Id = Guid.NewGuid(),
                LeafOnly = true,
                ScheduleCount = 10,
                ScheduleMode = ScheduleMode.ByCount,
                ScheduleShift = 3,
                ScheduleTimeSpan = 3,
                UserId = user.Id,
                VersionDate = DateTimeOffset.Now
            };
            context.Set<DB.Context.UserSettings>().Add(settings);
            await context.SaveChangesAsync();

            return user;
        }

        private async Task<IEnumerable<DB.Context.Formula>> AddFormulas(string nameMask, int count)
        {
            List<DB.Context.Formula> result = new List<DB.Context.Formula>();
            var context = _serviceProvider.GetRequiredService<DB.Context.DbPgContext>();
            for (int i = 0; i < count; i++)
            {
                var formula = CreateFormula(nameMask);
                context.Set<DB.Context.Formula>().Add(formula);
                await context.SaveChangesAsync();
                result.Add(formula);
            }
            return result;
        }

        private async Task<IEnumerable<DB.Context.Project>> AddProjects(string nameMask, Guid userId, int count)
        {
            List<DB.Context.Project> result = new List<DB.Context.Project>();
            var context = _serviceProvider.GetRequiredService<DB.Context.DbPgContext>();
            for (int i = 0; i < count; i++)
            {
                var project = CreateProject(nameMask, userId);
                context.Set<DB.Context.Project>().Add(project);
                await context.SaveChangesAsync();
                result.Add(project);
            }
            return result;
        }

        private async Task<DB.Context.Formula> AddFormula(string nameMask)
        {
            var context = _serviceProvider.GetRequiredService<DB.Context.DbPgContext>();
            var formula = CreateFormula(nameMask);
            context.Set<DB.Context.Formula>().Add(formula);
            await context.SaveChangesAsync();
            return formula;
        }

        private async Task<DB.Context.Project> AddProject(string nameMask, Guid userId)
        {
            var context = _serviceProvider.GetRequiredService<DB.Context.DbPgContext>();
            var project = CreateProject(nameMask, userId);
            context.Set<DB.Context.Project>().Add(project);
            await context.SaveChangesAsync();
            return project;
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

        private DB.Context.Formula CreateFormula (string nameMask)
        {
            var formula_id = Guid.NewGuid();
            return new DB.Context.Formula()
            {
                Name = string.Format(nameMask, formula_id),//$"formula_{formula_id}",
                Id = formula_id,
                IsDeleted = false,
                IsDefault = true,
                Text = "Min(SelectCount)",
                VersionDate = DateTimeOffset.Now
            };
        }

        private DB.Context.Project CreateProject(string nameMask, Guid userId)
        {
            var project_id = Guid.NewGuid();
            return new DB.Context.Project()
            {
                Name = string.Format(nameMask, project_id),//$"formula_{formula_id}",
                Id = project_id,
                IsDeleted = false,
                AddTime = 0,
                IsLeaf = false,
                LastUsedDate = DateTimeOffset.Now,
                Path = project_id.ToString(),
                Period = 60,
                Priority = 5000,
                UserId = userId,
                VersionDate = DateTimeOffset.Now
            };
        }
    }
}
