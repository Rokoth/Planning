using Contracts.Model.Schedule;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
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
    public class SheduleTests(CustomFixture fixture, ITestOutputHelper output) : IClassFixture<CustomFixture>
    {
        private readonly IServiceProvider _serviceProvider = fixture.ServiceProvider;

        /// <summary>
        /// 0.0.3.3.3
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SimpleTest()
        {
            var projPath = $"TestRun{DateTime.Now:yyyyMMddhhmmss}";
            Process mainProcess = null;            

            try
            {
                var formula = await AddFormula("default_formula_{0}");
                var user = await AddUser(formula.Id);

                BuildProject(projPath);
                ReplaceConfig(projPath);
                mainProcess = RunProject(projPath);
                var tryCount = 0;

               
            }
            catch (Exception ex)
            {
                output.WriteLine($"Exception while run test: {ex.Message} {ex.StackTrace}");
                throw;
            }
            finally
            {               
                if (mainProcess != null) StopProject(mainProcess);
                output.WriteLine($"Delete directory: {projPath}");
                //Directory.Delete(projPath, true);
            }
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
            output.WriteLine($"Run project");
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
            configJson["ConnectionStrings"]["MainConnection"] = fixture.ConnectionString;

            using (var writer = new StreamWriter(configFilePath, false))
            {
                writer.Write(configJson.ToString());
            }
        }

        private void BuildProject(string projPath)
        {
            output.WriteLine($"Build project to path: {projPath}");
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
            output.WriteLine(cmd.StandardOutput.ReadToEnd());
        }       
    }
}
