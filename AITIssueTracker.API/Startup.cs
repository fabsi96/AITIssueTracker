using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AITIssueTracker.API.Installer;
using AITIssueTracker.API.v0._1_Controller;
using AITIssueTracker.API.v0._2_Manager;
using AITIssueTracker.API.v0._3_DAL;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
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
            services.AddControllers()
                .AddNewtonsoftJson()
                .AddFluentValidation();

            services.InstallServicesInAssembly(Configuration);

            // === Dependency injection ===
            // TODO: Try-catch-finally in the database-context classes
            IConfigurationSection dbSection = Configuration.GetSection(PsqlSettings.KEY);
            PsqlSettings dbSettings = new PsqlSettings
            {
                DatabaseIp = "localhost",
                DatabasePort = 5432,
                Database = dbSection["dbname"],
                Password = dbSection["password"],
                Username = dbSection["user"]
            };
            services.AddSingleton(dbSettings);
            services.AddTransient<TestManager>();
            services.AddTransient<TestContext>();

            services.AddTransient<IProjectService, ProjectManager>();
            services.AddTransient<ProjectContext>();

            services.AddTransient<UserManager>();
            services.AddTransient<UserContext>();

            services.AddTransient<FeatureManager>();
            services.AddTransient<FeatureContext>();

            services.AddTransient<IssueManager>();
            services.AddTransient<IssueContext>();

            services.AddTransient<ViewManager>();
            services.AddTransient<ViewContext>();
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
                foreach (ApiVersionDescription endPoint in provider.ApiVersionDescriptions)
                {
                    config.SwaggerEndpoint($"/swagger/{endPoint.GroupName}/swagger.json", endPoint.GroupName.ToUpper());
                }
            });
        }
    }
}
