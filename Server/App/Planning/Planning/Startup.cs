using System;
using System.Linq;
using System.Threading.Tasks;
using Deploy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Planning.Common;
using Planning.DB.Context;
using Planning.Service;
using Microsoft.IdentityModel.Tokens;

namespace Planning
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CommonOptions>(Configuration);            
            services.AddControllersWithViews();
            services.AddDbContextPool<DbPgContext>((opt) =>
            {
                opt.EnableSensitiveDataLogging();
                var connectionString = Configuration.GetConnectionString("MainConnection");
                opt.UseNpgsql(connectionString);
            });
            services.AddScoped<DB.Repository.IRepository<User>, DB.Repository.Repository<User>>();   
            services.AddScoped<DB.Repository.IRepository<Formula>, DB.Repository.Repository<Formula>>();    
            services.AddScoped<DB.Repository.IRepository<UserSettings>, DB.Repository.Repository<UserSettings>>();
            services.AddScoped<DB.Repository.IRepository<Project>, DB.Repository.Repository<Project>>();
            services.AddScoped<DB.Repository.IRepository<Schedule>, DB.Repository.Repository<Schedule>>();
            services.AddScoped<DB.Repository.IRepository<AdditionalTask>, DB.Repository.Repository<AdditionalTask>>();

            services.AddScoped<DB.Repository.IRepository<UserHistory>, DB.Repository.Repository<UserHistory>>();
            services.AddScoped<DB.Repository.IRepository<FormulaHistory>, DB.Repository.Repository<FormulaHistory>>();
            services.AddScoped<DB.Repository.IRepository<UserSettingsHistory>, DB.Repository.Repository<UserSettingsHistory>>();
            services.AddScoped<DB.Repository.IRepository<ProjectHistory>, DB.Repository.Repository<ProjectHistory>>();
            services.AddScoped<DB.Repository.IRepository<ScheduleHistory>, DB.Repository.Repository<ScheduleHistory>>();

            services.AddScoped<IDeployService, DeployService>();
            services.AddScoped<IProjectSelectService, ProjectSelectService>();
            services.AddScoped<ICalculator, CalculatorNCalc>();
            services.AddScoped<IErrorNotifyService, ErrorNotifyService>();

            services.AddDataServices();
            services.ConfigureAutoMapper();
 
            services.AddCors();
            services.AddAuthentication()
            .AddJwtBearer("Token", options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //// укзывает, будет ли валидироваться издатель при валидации токена
                    ValidateIssuer = true,
                    //// строка, представляющая издателя
                    ValidIssuer = AuthOptions.ISSUER,

                    //// будет ли валидироваться потребитель токена
                    ValidateAudience = true,
                    //// установка потребителя токена
                    ValidAudience = AuthOptions.AUDIENCE,
                    //// будет ли валидироваться время существования
                    ValidateLifetime = true,

                    // установка ключа безопасности
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                    // валидация ключа безопасности
                    ValidateIssuerSigningKey = true,

                };
            }).AddCookie("Cookies", options => {
                options.LoginPath = new PathString("/Account/Login");
                options.LogoutPath = new PathString("/Account/Logout");
            });

            services
                .AddAuthorization(options =>
                {
                    var cookiePolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes("Cookies")
                        .Build();
                    options.AddPolicy("Cookie", cookiePolicy);
                    options.AddPolicy("Token", new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes("Token")
                        .Build());
                    options.DefaultPolicy = cookiePolicy;
                });
            services.AddSwaggerGen(s =>
            {
                s.OperationFilter<AddRequiredHeaderParameter>();
            });

            services.AddHostedService<BuildScheduleHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
