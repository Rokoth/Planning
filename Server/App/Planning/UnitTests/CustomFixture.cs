using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Serilog;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Planning.Common;
using Planning.Service;
using Deploy;
using Planning.DB.Context;
using System.Security.Cryptography;
using System.Text;

namespace Planning.UnitTests
{
    public class CustomFixture : IDisposable
    {
        public string ConnectionString { get; private set; }
        public string RootConnectionString { get; private set; }
        public string DatabaseName { get; private set; }
        public ServiceProvider ServiceProvider { get; private set; }

        public CustomFixture()
        {

            Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Verbose()
             .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "test-log.txt"))
             .CreateLogger();

            var serviceCollection = new ServiceCollection();
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();

            DatabaseName = $"planning_test_{DateTimeOffset.Now:yyyyMMdd_hhmmss}";
            ConnectionString = Regex.Replace(config.GetConnectionString("MainConnection"), "Database=.*?;", $"Database={DatabaseName};");
            RootConnectionString = Regex.Replace(config.GetConnectionString("MainConnection"), "Database=.*?;", $"Database=postgres;");
            serviceCollection.Configure<CommonOptions>(config);
            serviceCollection.AddLogging(configure => configure.AddSerilog());
            serviceCollection.AddDataServices();
            serviceCollection.AddScoped<IDeployService, DeployService>();
            serviceCollection.AddScoped<IAuthService, AuthService>();
            serviceCollection.AddScoped<IErrorNotifyService, ErrorNotifyService>();            

            serviceCollection.AddDbContext<DB.Context.DbPgContext>(opt => opt.UseNpgsql(ConnectionString));
            serviceCollection.AddScoped<DB.Repository.IRepository<DB.Context.User>, DB.Repository.Repository<DB.Context.User>>();
            serviceCollection.AddScoped<DB.Repository.IRepository<DB.Context.Formula>, DB.Repository.Repository<DB.Context.Formula>>();
            //serviceCollection.AddScoped<IRepository<Client>, Repository<Client>>();
            //serviceCollection.AddScoped<IRepositoryHistory<UserHistory>, RepositoryHistory<UserHistory>>();
            //serviceCollection.AddScoped<IRepositoryHistory<ClientHistory>, RepositoryHistory<ClientHistory>>();

            serviceCollection.ConfigureAutoMapper();
            ServiceProvider = serviceCollection.BuildServiceProvider();

            ServiceProvider.GetRequiredService<IOptions<CommonOptions>>().Value.ConnectionString = ConnectionString;
            ServiceProvider.GetRequiredService<IDeployService>().Deploy().GetAwaiter().GetResult();

        }

        public void Dispose()
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<CustomFixture>>();
            try
            {
                using NpgsqlConnection _connPg = new NpgsqlConnection(RootConnectionString);
                _connPg.Open();
                string script1 = "SELECT pg_terminate_backend (pg_stat_activity.pid) " +
                    $"FROM pg_stat_activity WHERE pid<> pg_backend_pid() AND pg_stat_activity.datname = '{DatabaseName}'; ";
                var cmd1 = new NpgsqlCommand(script1, _connPg);
                cmd1.ExecuteNonQuery();

                string script2 = $"DROP DATABASE {DatabaseName};";
                var cmd2 = new NpgsqlCommand(script2, _connPg);
                cmd2.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                logger.LogError($"Error on dispose: {ex.Message}; StackTrace: {ex.StackTrace}");
            }
        }

        public Formula CreateFormula()
        {
            var id = Guid.NewGuid();
            var formula = new Formula()
            {
                Id = id,
                IsDefault = true,
                IsDeleted = false,
                Name = "TestFormula",
                Text = "Min(SelectCount)",
                VersionDate = DateTimeOffset.Now
            };
            return formula;
        }

        public User CreateUser(string nameMask, string descriptionMask, string loginMask, string passwordMask, Guid formulaId)
        {

            var id = Guid.NewGuid();
            var user = new User()
            {
                Name = string.Format(nameMask, id),
                Id = id,
                Description = string.Format(descriptionMask, id),
                IsDeleted = false,
                Login = string.Format(loginMask, id),
                Password = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format(passwordMask, id))),
                VersionDate = DateTimeOffset.Now,
                FormulaId = formulaId
            };
            return user;
        }
    }
}
