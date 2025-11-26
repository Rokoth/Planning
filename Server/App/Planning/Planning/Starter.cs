using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Planning.Common;
using Serilog;

namespace Planning
{
    public class Starter
    {
        private IWebHost webHost;
        private readonly string[] Args;
        private ILogger _logger;

        public Starter(ILogger logger, string[] args)
        {
            _logger = logger;
            Args = args;
        }

        public bool Start()
        {
            try
            {
                webHost = BuildWebHost(GetWebHostBuilder(Args));
                _logger.Information("Start service...");
                webHost.Start();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Starting service error! \nException:\n {ex.Message} \nStackTrace:\n {ex.StackTrace} ");
                throw;
            }
        }

        public bool Stop()
        {
            webHost?.StopAsync().ContinueWith(s =>
            {
                if (s.IsFaulted)
                {
                    _logger.Error($"Stopping service error! \nException:\n {s.Exception}");
                }
                webHost?.Dispose();
            });
            return true;
        }

        private IWebHost BuildWebHost(IWebHostBuilder webHostBuilder)
        {
            return webHostBuilder.UseStartup<Startup>()
                .Build();
        }

        protected IWebHostBuilder GetWebHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(
                    new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddDbConfiguration()
                    .Build())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureLogging((hostingContext, logging) => CreateLogger(hostingContext, logging))
                .UseKestrel();

            return builder;
        }

        /// <summary>
        /// Create Logger method
        /// </summary>
        /// <param name="hostingContext"></param>
        /// <param name="logging"></param>
        private static void CreateLogger(WebHostBuilderContext hostingContext, Microsoft.Extensions.Logging.ILoggingBuilder logging)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(hostingContext.Configuration)
                .CreateLogger();
            logging.AddSerilog(Log.Logger);
            logging.AddErrorNotifyLogger(config =>
            {
                config.Options = hostingContext.Configuration
                    .GetSection("ErrorNotifyOptions")
                    .Get<ErrorNotifyOptions>();
            });
        }
    }
}
