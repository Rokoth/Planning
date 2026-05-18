using Contracts.Model.Schedule;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Planning.UnitTests
{
    /// <summary>
    /// api unit tests
    /// </summary>
    public class WebIntegrtationTests : IClassFixture<CustomFixture>
    {
        private ITestOutputHelper _output;
        private readonly IServiceProvider _serviceProvider;
        private CustomFixture _fixture;

        public WebIntegrtationTests(CustomFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _serviceProvider = fixture.ServiceProvider;
            _output = output;
        }

        /// <summary>
        /// AuthController. Test for Auth method (positive scenario)
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ApiAuthTest()
        {
            var projPath = $"TestRun{DateTime.Now:yyyyMMddhhmmss}";
            Process mainProcess = null;
            IWebDriver driver = null;


            try
            {
                var formula = await AddFormula("default_formula_{0}");
                var user = await AddUser(formula.Id);

                BuildProject(projPath);
                ReplaceConfig(projPath);
                mainProcess = RunProject(projPath);
                var tryCount = 0;

                //todo: переделать на playwright
                while (true)
                {
                    try
                    {
                        driver = new ChromeDriver();
                        driver.Manage().Window.Maximize();
                        driver.Navigate().GoToUrl("https://localhost:5721/");
                        Assert.True(driver.Url.Contains("localhost"), "Что-то не так =(");
                        break;
                    }
                    catch (Exception)
                    {
                        if (tryCount == 10) throw;
                        tryCount++;
                    }
                }


                Authorization(driver, user.Id, false);
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Exception while run test: {ex.Message} {ex.StackTrace}");
                throw;
            }
            finally
            {
                if (driver != null) driver.Quit();
                if (mainProcess != null) StopProject(mainProcess);
                _output.WriteLine($"Delete directory: {projPath}");
                //Directory.Delete(projPath, true);
            }
        }

        ///// <summary>
        ///// FormulaController. Test for Update method (positive scenario)
        ///// </summary>
        ///// <returns></returns>
        //[Fact]
        //public async Task FormulaUpdateTest()
        //{
        //    var formula = await AddFormula("default_formula_{0}");
        //    var user = await AddUser(formula.Id);
        //    var identity = await AuthAndAssert(user);

        //    var testFormula = await AddFormula("formula_{0}");
        //    var newName = testFormula.Name + "_changed";
        //    FormulaApiController controller = new FormulaApiController(_serviceProvider);            
        //    var res = await controller.Update(new FormulaUpdater()
        //    { 
        //       Id = testFormula.Id,
        //       Name = newName,
        //       Text = testFormula.Text
        //    });
        //    Assert.True(res is OkObjectResult);
        //    var result = res as OkObjectResult;
        //    var changed = JObject.FromObject(result.Value).ToObject<Formula>();
        //    Assert.Equal(newName, changed.Name);

        //    var context = _serviceProvider.GetRequiredService<DB.Context.DbPgContext>();
        //    var actual = context.Formulas.Where(s => s.Id == testFormula.Id).FirstOrDefault();
        //    Assert.Equal(newName, actual.Name);
        //}

        ///// <summary>
        ///// FormulaController. Test for Add method (positive scenario)
        ///// </summary>
        ///// <returns></returns>
        //[Fact]
        //public async Task FormulaAddTest()
        //{
        //    var formula = await AddFormula("default_formula_{0}");
        //    var user = await AddUser(formula.Id);
        //    var identity = await AuthAndAssert(user);

        //    var testName = $"formula_{Guid.NewGuid()}";
        //    FormulaApiController controller = new FormulaApiController(_serviceProvider);
        //    var res = await controller.Create(new FormulaCreator()
        //    {               
        //        Name = testName,
        //        Text = ""
        //    });
        //    Assert.True(res is OkObjectResult);
        //    var result = res as OkObjectResult;
        //    var changed = JObject.FromObject(result.Value).ToObject<Formula>();
        //    Assert.Equal(testName, changed.Name);

        //    var context = _serviceProvider.GetRequiredService<DB.Context.DbPgContext>();
        //    var actual = context.Formulas.Where(s => s.Id == changed.Id).FirstOrDefault();
        //    Assert.Equal(testName, actual.Name);
        //}

        ///// <summary>
        ///// FormulaController. Test for Get method (positive scenario)
        ///// </summary>
        ///// <returns></returns>
        //[Fact]
        //public async Task FormulaGetTest()
        //{
        //    var formula = await AddFormula("default_formula_{0}");
        //    var user = await AddUser(formula.Id);
        //    var identity = await AuthAndAssert(user);

        //    await AddFormulas("formula_select_{0}", 10);
        //    await AddFormulas("formula_not_select_{0}", 10);
        //    FormulaApiController controller = new FormulaApiController(_serviceProvider);
        //    var res = await controller.Get("formula_select", 10, 0, null);
        //    Assert.True(res is OkObjectResult);
        //    var result = res as OkObjectResult;
        //    var actuals = JArray.FromObject(result.Value);
        //    Assert.Equal(10, actuals.Count);
        //    foreach (var assert in actuals)
        //    {
        //        var actual = assert.ToObject<Formula>();
        //        Assert.Contains("formula_select", actual.Name);
        //    }           
        //}

        ///// <summary>
        ///// FormulaController. Test for Get method (positive scenario)
        ///// </summary>
        ///// <returns></returns>
        //[Fact]
        //public async Task FormulaGetItemTest()
        //{
        //    var formula = await AddFormula("default_formula_{0}");
        //    var user = await AddUser(formula.Id);
        //    var identity = await AuthAndAssert(user);

        //    var testFormula = await AddFormula("formula_select_{0}");
            
        //    FormulaApiController controller = new FormulaApiController(_serviceProvider);
        //    var res = await controller.GetItem(testFormula.Id);
        //    Assert.True(res is OkObjectResult);
        //    var result = res as OkObjectResult;
        //    var actual = result.Value as Formula;
        //    Assert.Equal(testFormula.Id, actual.Id);
        //}

        ///// <summary>
        ///// ProjectController. Test for GetItem method (positive scenario)
        ///// </summary>
        ///// <returns></returns>
        //[Fact]
        //public async Task ProjectGetItemTest()
        //{
        //    var formula = await AddFormula("default_formula_{0}");
        //    var user = await AddUser(formula.Id);
        //    var identity = await AuthAndAssert(user);

        //    var testProject = await AddProject("project_select_{0}", user.Id);

        //    ProjectApiController controller = new(_serviceProvider);
        //    var res = await controller.GetItem(testProject.Id);

        //    Assert.True(res is OkObjectResult);
        //    var result = res as OkObjectResult;
        //    var actual = result.Value as Project;
        //    Assert.Equal(testProject.Id, actual.Id);
        //}

        ///// <summary>
        ///// ProjectController. Test for Get method (positive scenario)
        ///// </summary>
        ///// <returns></returns>
        //[Fact]
        //public async Task ScheduleGetTest()
        //{
        //    ScheduleApiController controller = new ScheduleApiController(_serviceProvider);
        //    var formula = await AddFormula("default_formula_{0}");
        //    var user = await AddUser(formula.Id);
        //    var identity = await AuthAndAssert(user);
        //    var direction = await AddDirection(user.Id);
        //    var projects = await AddProjects("project{0}", user.Id, 10);
            
        //    var schedProject = projects.FirstOrDefault();
        //    var schedules = await AddSchedule(schedProject.Id, user.Id);
           
        //    var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
        //                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //                                new Claim(ClaimTypes.Name, user.Id.ToString())
        //                           }, "TestAuthentication"));

        //    controller.ControllerContext = new ControllerContext();
        //    controller.ControllerContext.HttpContext = new DefaultHttpContext { User = claims };

        //    var res = await controller.Get("project_select", size: 10, page: 0);
        //    Assert.True(res is OkObjectResult);
        //    var result = res as OkObjectResult;
        //    var actuals = JArray.FromObject(result.Value);
        //    Assert.Equal(10, actuals.Count);
        //    foreach (var assert in actuals)
        //    {
        //        var actual = assert.ToObject<Project>();
        //        Assert.Contains("project_select", actual.Name);
        //    }
        //}

        ///// <summary>
        ///// ScheduleController. Test for Add method (positive scenario)
        ///// </summary>
        ///// <returns></returns>
        //[Fact]
        //public async Task ScheduleAddTest()
        //{
        //    var formula = await AddFormula("default_formula_{0}");
        //    var user = await AddUser(formula.Id);
        //    var identity = await AuthAndAssert(user);

        //    var testProject = await AddProject("project_select_{0}", user.Id);

        //    ScheduleApiController controller = new(_serviceProvider);

        //    var claims = new ClaimsPrincipal(new ClaimsIdentity([
        //                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //                                new Claim(ClaimTypes.Name, user.Id.ToString())
        //                           ], "TestAuthentication"));

        //    controller.ControllerContext = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext { User = claims }
        //    };

        //    var res = await controller.Create(new ScheduleCreator()
        //    {
        //        BeginDate = DateTime.Now,
        //        ProjectId = testProject.Id,
        //        SetBeginDate = true,
        //        UserId = user.Id,
        //    });

        //    Assert.True(res is OkObjectResult);
        //    var result = res as OkObjectResult;
        //    var actual = result.Value as Schedule;
        //    Assert.NotNull(actual);

        //    var context = _serviceProvider.GetRequiredService<DB.Context.DbPgContext>();
        //    var dbEntry = await context.Set<DB.Context.Schedule>().FirstOrDefaultAsync(s => s.Id == actual.Id);
        //    Assert.NotNull(dbEntry);
        //}

        ///// <summary>
        ///// ProjectController. Test for Get method (positive scenario)
        ///// </summary>
        ///// <returns></returns>
        //[Fact]
        //public async Task ProjectGetTest()
        //{



        //    ProjectApiController controller = new ProjectApiController(_serviceProvider);
        //    var formula = await AddFormula("default_formula_{0}");
        //    var user = await AddUser(formula.Id);
        //    var identity = await AuthAndAssert(user);

        //    await AddProjects("project_select_{0}", user.Id, 10);
        //    await AddProjects("project_not_select_{0}", user.Id, 10);

        //    var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
        //                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //                                new Claim(ClaimTypes.Name, user.Id.ToString())
        //                           }, "TestAuthentication"));

        //    controller.ControllerContext = new ControllerContext();
        //    controller.ControllerContext.HttpContext = new DefaultHttpContext { User = claims };

        //    var res = await controller.Get("project_select", size: 10, page: 0);
        //    Assert.True(res is OkObjectResult);
        //    var result = res as OkObjectResult;
        //    var actuals = JArray.FromObject(result.Value);
        //    Assert.Equal(10, actuals.Count);
        //    foreach (var assert in actuals)
        //    {
        //        var actual = assert.ToObject<Project>();
        //        Assert.Contains("project_select", actual.Name);
        //    }
        //}

                
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

        private async Task<DB.Context.Direction> AddDirection(Guid userId, Guid categoryId)
        {
            var context = _serviceProvider.GetRequiredService<DB.Context.DbPgContext>();
            var direction = CreateDirection(userId, categoryId);
            context.Set<DB.Context.Direction>().Add(direction);
            await context.SaveChangesAsync();

            return direction;
        }

        private async Task<DB.Context.Schedule> AddSchedule(Guid project, Guid directionId, Guid userId)
        {
            var beginDate = DateTimeOffset.Now;            
            
            var context = _serviceProvider.GetRequiredService<DB.Context.DbPgContext>();
            var schedule = CreateSchedule(project, directionId, userId, beginDate, true);
            context.Set<DB.Context.Schedule>().Add(schedule);
            await context.SaveChangesAsync();
            
            return schedule;
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

        private DB.Context.Schedule CreateSchedule(Guid projectId, Guid directionId, Guid userId, DateTimeOffset date, bool isRunning)
        {
            var project_id = Guid.NewGuid();
            return new DB.Context.Schedule()
            {                
                Id = project_id,
                IsDeleted = false,
                AddTime = 0,               
                UserId = userId,
                VersionDate = DateTimeOffset.Now,
                BeginDate = date,
                DirectionId = directionId,
                EndDate = date.AddHours(1),
                IsClosed = false,
                IsRunning = isRunning,
                ProjectId = projectId                
            };
        }
        private DB.Context.Direction CreateDirection(Guid userId, Guid categoryId)
        {
            var id = Guid.NewGuid();
            return new DB.Context.Direction()
            {
                Id = id,
                IsDeleted = false,               
                UserId = userId,
                VersionDate = DateTimeOffset.Now,
                BeginDate = DateTime.Now,
                Description = "test_direction",
                DirectionCategoryId = categoryId,
                Name = "test_direction",
                Priority = 5000
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

        private Process RunProject(string projPath)
        {
            _output.WriteLine($"Run project");
            Process cmd = new Process();
            cmd.StartInfo.FileName = $"{projPath}\\TaskCollector.exe";
            cmd.Start();
            return cmd;
        }


        private void StopProject(Process cmd)
        {
            try
            {
                cmd?.Kill();
                cmd?.Close();
                cmd?.WaitForExit(10000);
            }
            catch
            {

            }
        }

        private void ReplaceConfig(string projPath)
        {
            var configFilePath = Path.Combine(projPath, "appsettings.json");
            string config = "";
            using (var stream = new StreamReader(configFilePath))
            {
                config = stream.ReadToEnd();
            }
            var configJson = JObject.Parse(config);
            configJson["ConnectionStrings"]["MainConnection"] = _fixture.ConnectionString;

            using (var writer = new StreamWriter(configFilePath, false))
            {
                writer.Write(configJson.ToString());
            }
        }

        private void BuildProject(string projPath)
        {
            _output.WriteLine($"Build project to path: {projPath}");
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            var command = $"dotnet build ..\\..\\..\\..\\Planning\\Planning.csproj -o {projPath}";

            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit(10000);
            _output.WriteLine(cmd.StandardOutput.ReadToEnd());
        }

        private static void Authorization(IWebDriver driver, Guid userId, bool falseAuth)
        {
            var authButton = driver.FindElement(By.Id("AuthButton"));
            authButton.Click();
            var loginField = driver.FindElement(By.Id("Login"));
            loginField.SendKeys($"user_login_{userId}");
            var passwordField = driver.FindElement(By.Id("Password"));
            if (falseAuth)
            {
                passwordField.SendKeys($"wrong_password");
            }
            else
            {
                passwordField.SendKeys($"user_password_{userId}");
            }
            var enterButton = driver.FindElement(By.Id("EnterButton"));
            enterButton.Click();
            if (falseAuth)
            {
                Assert.True(driver.Url.Contains("Error"), "Авторизация неудачна");
            }
            else
            {
                Assert.False(driver.Url.Contains("Error"), "Авторизация неудачна");
            }
        }
    }
}
