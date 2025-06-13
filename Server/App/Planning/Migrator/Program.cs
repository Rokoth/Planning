using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Migrator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseConfiguration(
                        new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables().Build())
                    .ConfigureLogging((hostingContext, logging) =>
                    {
                        Log.Logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(hostingContext.Configuration)
                            .CreateLogger();
                        logging.AddSerilog(Log.Logger);
                    })
                    .UseKestrel()
                    .UseStartup<Startup>();
                })
                .UseSerilog()
                .UseConsoleLifetime();

            hostBuilder.Build().Run();
        }

        
    }
}
