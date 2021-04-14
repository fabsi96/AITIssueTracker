using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AITIssueTracker.API.Installer.SwaggerConfig
{
    public class SwaggerInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                services.AddSwaggerGenNewtonsoftSupport();

                services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerHeader>();

                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                services.AddSwaggerExamplesFromAssemblies(assemblies);

                services.AddSwaggerGen(options =>
                {
                    options.OperationFilter<SwaggerBody>();

                    options.EnableAnnotations();

                    options.ExampleFilters();

                    string xmlSwaggerFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    string xmlSwaggerDocumentationPath = Path.Combine(AppContext.BaseDirectory, xmlSwaggerFilename);
                    Console.WriteLine($"Swagger file path: {xmlSwaggerDocumentationPath}");
                    options.IncludeXmlComments(xmlSwaggerDocumentationPath);
                });

            }
            catch (Exception e)
            {
                Console.WriteLine($"InstallServics: Error: {e.Message}");
            }
        }
    }
}
