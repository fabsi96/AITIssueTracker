using System;
using System.Collections.Generic;
using System.Text;
using AITIssueTracker.Model.v0._1_FormModel;
using Swashbuckle.AspNetCore.Filters;

namespace AITIssueTracker.Model.v0._4_Examples
{
    public class ProjectExample : IExamplesProvider<ProjectForm>
    {
        public ProjectForm GetExamples()
        {
            return new ProjectForm
            {
                Title = "Issuetracker",
                Description = "Verwaltung von Aufgaben zu einem Projekt.",
                IsDone = false
            };
        }
    }
}
