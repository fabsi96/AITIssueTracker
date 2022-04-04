using AITIssueTracker.API.Installer;
using AITIssueTracker.API.v0._1_Controller;
using AITIssueTracker.API.v0._2_Manager;
using AITIssueTracker.API.v0._2_Manager.Contracts;
using AITIssueTracker.API.v0._3_DAL;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace AITIssueTracker.API
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {   
            // Enable comptroller's
            services.AddControllers(options =>
                {
                    // options.Filters.Add<SomeFilterName>()
                })
                .AddFluentValidation()
                .AddNewtonsoftJson(setup => setup.SerializerSettings.Converters.Add(new StringEnumConverter()));

            services.InstallServicesInAssembly(Configuration);
            services.AddDbContext<DataDb>();
            services.AddDbContext<DbTestContext>();
            services.AddAutoMapper(typeof(Startup));
            
            // === Dependency injection ===
            var dbSection = Configuration.GetSection(PsqlSettings.KEY);
            var dbSettings = new PsqlSettings
            {
                DatabaseIp = "localhost",
                DatabasePort = 5432,
                Database = dbSection["dbname"],
                Password = dbSection["password"],
                Username = dbSection["user"]
            };
            
            services.AddSingleton(dbSettings);

            services.AddTransient<IUserService, UserService>();
            
            // services.AddScoped<IProjectService, ProjectService>();
            services.AddTransient<ProjectManager>();
            services.AddSingleton<ProjectContext>();

            // services.AddTransient<IFeatureService, FeatureService>();
            services.AddTransient<FeatureManager>();
            services.AddTransient<FeatureContext>();

            services.AddTransient<IssueManager>();
            services.AddTransient<IssueContext>();

            services.AddTransient<ViewManager>();
            services.AddTransient<ViewContext>();

            services.AddTransient<GithubContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // User url routing
            app.UseRouting();
            
            // Map urls to controller
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Swagger documentation
            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.DocExpansion(DocExpansion.None);
                foreach (var endPoint in provider.ApiVersionDescriptions)
                {
                    config.SwaggerEndpoint($"/swagger/{endPoint.GroupName}/swagger.json", endPoint.GroupName.ToUpper());
                }
            });
        }
    }
}
