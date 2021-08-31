//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Deployer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Planning.Common;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Deploy
{

    /// <summary>
    /// Wrapper for deploy lib
    /// </summary>
    public class DeployService : IDeployService
    {
        private readonly ILogger<DeployService> _logger;
        private readonly string _connectionString;
        private readonly IErrorNotifyService errorNotifyService;

        /// <summary>
        /// ctor from container
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DeployService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<DeployService>>();
            var _options = serviceProvider.GetRequiredService<IOptions<CommonOptions>>();
            _connectionString = _options.Value.ConnectionString;
            errorNotifyService = serviceProvider.GetRequiredService<IErrorNotifyService>();
        }

        /// <summary>
        /// ctor with connectionString
        /// </summary>
        /// <param name="connectionString"></param>
        public DeployService(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Deploy method
        /// </summary>
        /// <param name="num">last update num</param>
        /// <returns></returns>
        public async Task Deploy(int? num = null)
        {
            var deployLog = string.Empty;
            try
            {
                CheckDbForExists(_connectionString);

                DeploySettings deploySettings = new DeploySettings()
                {
                    BeginNum = num,
                    ConnectionString = _connectionString,
                    CheckSqlPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "Check"),
                    DeploySqlPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "Deploy"),
                    UpdateSqlPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "Update")
                };
                var deployer = new Deployer.Deployer(deploySettings);
                deployer.OnError += (sender, message) =>
                {
                    deployLog += message + "\r\n";
                    _logger?.LogError(message);
                };
                deployer.OnDebug += (sender, message) =>
                {
                    _logger?.LogDebug(message);
                };
                deployer.OnMessage += (sender, message) =>
                {
                    _logger?.LogInformation(message);
                };
                deployer.OnWarning += (sender, message) =>
                {
                    deployLog += message + "\r\n";
                    _logger?.LogWarning(message);
                };

                if (!await deployer.Deploy())
                {
                    throw new DeployException($"DB was not deploy, log: {deployLog}");
                }
            }
            catch (DeployException ex)
            {
                await errorNotifyService.Send($"Error while Deploy: {ex.Message} {ex.StackTrace}");
                throw;
            }
            catch (Exception ex)
            {
                await errorNotifyService.Send($"Error while Deploy DB.\r\n" +
                    $"Message: {ex.Message}\r\n" +
                    $"StackTrace: {ex.StackTrace}\r\n" +
                    $"DeployLog: {deployLog}");
                throw new DeployException(
                    $"Error while Deploy DB.\r\n" +
                    $"Message: {ex.Message}\r\n" +
                    $"StackTrace: {ex.StackTrace}\r\n" +
                    $"DeployLog: {deployLog}");
            }
        }

        /// <summary>
        /// todo: move to deploy lib
        /// </summary>
        /// <param name="connectionString"></param>
        private void CheckDbForExists(string connectionString)
        {
            try
            {
                var dbName = Regex.Match(connectionString, "Database=(.*?);").Groups[1].Value;
                var rootConnectionString = Regex.Replace(connectionString, "Database=.*?;", $"Database=postgres;");
                using NpgsqlConnection _connPg = new NpgsqlConnection(rootConnectionString);
                _connPg.Open();
                string script1 = $"select exists(SELECT 1 FROM pg_database WHERE datname = '{dbName}');";
                var cmd1 = new NpgsqlCommand(script1, _connPg);
                if (!(bool)cmd1.ExecuteScalar())
                {
                    string script2 = $"create database {dbName};";
                    var cmd2 = new NpgsqlCommand(script2, _connPg);
                    cmd2.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new DeployException($"Не удалось развернуть базу данных: " +
                    $"ошибка при проверке или создании базы: {ex.Message} {ex.StackTrace}");
            }
        }
    }
}
