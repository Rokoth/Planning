using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Configuration;

using Planning.Common;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Planning
{
    public static class CustomExtensionMethods
    {
        public static IConfigurationBuilder AddDbConfiguration(this IConfigurationBuilder builder)
        {
            var configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("MainConnection");
            builder.AddConfigDbProvider(options => options.UseNpgsql(connectionString), connectionString);
            return builder;
        }

        public static IConfigurationBuilder AddConfigDbProvider(
            this IConfigurationBuilder configuration, Action<DbContextOptionsBuilder> setup, string connectionString)
        {
            configuration.Add(new ConfigDbSource(setup, connectionString));
            return configuration;
        }

        public static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
        {
            var mappingConfig = new AutoMapper.MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));

            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }

        public static Microsoft.Extensions.Logging.ILoggingBuilder AddErrorNotifyLogger(
        this Microsoft.Extensions.Logging.ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<Microsoft.Extensions.Logging.ILoggerProvider, ErrorNotifyLoggerProvider>());

            LoggerProviderOptions.RegisterProviderOptions
                <ErrorNotifyLoggerConfiguration, ErrorNotifyLoggerProvider>(builder.Services);

            return builder;
        }

        public static Microsoft.Extensions.Logging.ILoggingBuilder AddErrorNotifyLogger(
            this Microsoft.Extensions.Logging.ILoggingBuilder builder,
            Action<ErrorNotifyLoggerConfiguration> configure)
        {
            builder.Services.Configure(configure);
            builder.AddErrorNotifyLogger();

            return builder;
        }
    }
}
