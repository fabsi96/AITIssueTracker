using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace AITIssueTracker.API.Installer.SwaggerConfig
{
    public static class SwaggerDescription
    {
        public static string GetVersionedDescription(this ApiVersionDescription apiVersionDescription)
        {
            try
            {
                string targetVersion = apiVersionDescription.ApiVersion.ToString();

                // ReSharper disable once PossibleNullReferenceException
                foreach (Type projClass in Assembly.GetEntryAssembly()?.GetTypes())
                {
                    ApiVersionAttribute classVersionAttribute = projClass.GetCustomAttribute<ApiVersionAttribute>();
                    bool classIsVersionDescription =
                        projClass.GetCustomAttribute<VersionedDescriptionAttribute>() != null;
                    if (classVersionAttribute == null ||
                        !classIsVersionDescription)
                        continue;
                    foreach (ApiVersion classVersion in classVersionAttribute.Versions)
                    {
                        if (!targetVersion.Equals(classVersion.ToString()))
                        {
                            continue;
                        }

                        // Found correct file to load summary from it
                        string versionSummary = string.Empty; // projClass.GetVersionDocumentation();
                        return string.IsNullOrEmpty(versionSummary) ?
                            $"Default description for version {targetVersion}." :
                            versionSummary;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return string.Empty;
        }
    }
}
