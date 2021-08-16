using Microsoft.Extensions.DependencyInjection;
using Planning.DB.Context;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Planning.UnitTests
{
    public class DeployTest : IClassFixture<CustomFixture>
    {
        private readonly IServiceProvider _serviceProvider;
        private CustomFixture _fixture;

        public DeployTest(CustomFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
            _fixture = fixture;
        }

        [Fact]
        public async Task DeployedTest()
        {
            var context = _serviceProvider.GetRequiredService<DbPgContext>();

            var formula = _fixture.CreateFormula();
            var user = _fixture.CreateUser("user_{0}", "user_description_{0}", "user_login_{0}", "user_password_{0}", formula.Id);
            var settings = new UserSettings()
            {
                DefaultProjectTimespan = 1,
                Id = Guid.NewGuid(),
                IsDeleted = false,
                LeafOnly = true,
                ScheduleMode = ScheduleMode.Manual,
                ScheduleShift = 1,
                UserId = user.Id,
                VersionDate = DateTimeOffset.Now
            };
            var project = new Project()
            {
                AddTime = 0,
                Id = Guid.NewGuid(),
                IsDeleted = false,
                IsLeaf = true,
                Name = "test",
                ParentId = null,
                Path = "test",
                Priority = 1,
                UserId = user.Id,
                VersionDate = DateTimeOffset.Now
            };
            var schedule = new Schedule()
            {
                VersionDate = DateTimeOffset.Now,
                Id = Guid.NewGuid(),
                BeginDate = DateTimeOffset.Now,
                EndDate = DateTimeOffset.Now,
                IsDeleted = false,
                IsRunning = false,
                Order = 1,
                ProjectId = project.Id,
                UserId = user.Id
            };

            context.Settings.Add(new Settings() { Id = 100, ParamName = "test", ParamValue = "test" });
            context.Formulas.Add(formula);
            context.Users.Add(user);
            context.UserSettings.Add(settings);
            context.Projects.Add(project);
            context.Schedules.Add(schedule);

            await context.SaveChangesAsync();

            Assert.NotNull(context.Settings.FirstOrDefault());
            Assert.NotNull(context.Formulas.FirstOrDefault());
            Assert.NotNull(context.Users.FirstOrDefault());
            Assert.NotNull(context.UserSettings.FirstOrDefault());
            Assert.NotNull(context.Projects.FirstOrDefault());
            Assert.NotNull(context.Schedules.FirstOrDefault());

            Assert.NotNull(context.FormulaHistories.FirstOrDefault());
            Assert.NotNull(context.UserHistories.FirstOrDefault());
            Assert.NotNull(context.UserSettingsHistories.FirstOrDefault());
            Assert.NotNull(context.ProjectHistories.FirstOrDefault());
            Assert.NotNull(context.ScheduleHistories.FirstOrDefault());
        }
    }
}
