using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AITIssueTracker.API.Installer.VersioningConfig
{
    public class VersioningInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                Console.WriteLine($"Install versioning!");
                services.AddApiVersioning(options =>
                {
                    // Default version properties
                    // options.DefaultApiVersion = new ApiVersion(0, 0);
                    // options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = ApiVersionReader.Combine(
                        new HeaderApiVersionReader()
                        {
                            HeaderNames = {"api-version"},
                        },
                        new QueryStringApiVersionReader()
                        {
                            ParameterNames = {"api-version "},
                        });
                });
                services.AddVersionedApiExplorer(options =>
                {
                    // Version Deklaration Format
                    options.GroupNameFormat = "'v'VVV";

                    // API Version in URL
                    options.SubstituteApiVersionInUrl = true;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"InstallServices: Error: {e.Message}");
            }
        }
    }
}
