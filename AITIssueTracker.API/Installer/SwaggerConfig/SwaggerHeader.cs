using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AITIssueTracker.API.Installer.SwaggerConfig
{
    public class SwaggerHeader : IConfigureOptions<SwaggerGenOptions>
    {
        private IApiVersionDescriptionProvider Provider { get; }

        public SwaggerHeader(IApiVersionDescriptionProvider provider)
        {
            Provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (ApiVersionDescription description in Provider.ApiVersionDescriptions)
            {
                Console.WriteLine($"Create swagger documentation of {description.GroupName}");
                options.SwaggerDoc(description.GroupName, CreateInfoForVersions(description));
            }
        }

        private OpenApiInfo CreateInfoForVersions(ApiVersionDescription description)
        {
            try
            {
                Uri contactUrl = new UriBuilder("127.0.0.1").Uri;
                Uri licenseUrl = new UriBuilder("127.0.0.1").Uri;
                Uri serviceUrl = new UriBuilder("127.0.0.1").Uri;
                return new OpenApiInfo
                {
                    Title = $"API {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    Description = description.IsDeprecated ? "This version is deprecated." : description.GetVersionedDescription(),
                    Contact = new OpenApiContact { Name = "Contact", Url = contactUrl, Email = "email" },
                    License = new OpenApiLicense { Name = "License", Url = licenseUrl },
                    TermsOfService = serviceUrl
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error building Info for Version {description.GroupName}." +
                                  $"\n{e.Message}");
                return new OpenApiInfo() { Title = "Error" };
            }
        }
    }
}
