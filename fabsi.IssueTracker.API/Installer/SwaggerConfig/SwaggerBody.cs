using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AITIssueTracker.API.Installer.SwaggerConfig
{
    public class SwaggerBody : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters is null)
            {
                Console.WriteLine($"Apply: No params for swagger.");
                return;
            }

            ApiDescription apiDescription = context.ApiDescription;
            operation.Deprecated = apiDescription.IsDeprecated();
            foreach (OpenApiParameter parameter in operation.Parameters)
            {
                ApiParameterDescription description = apiDescription.ParameterDescriptions.First(param => param.Name == parameter.Name);
                parameter.Description ??= description.ModelMetadata?.Description;

                if (parameter.Schema.Default == null && description.DefaultValue != null)
                {
                    parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());
                }

                parameter.Required |= description.IsRequired;
            }
        }
    }
}
