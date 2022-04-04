using System;
using System.Collections.Generic;
using System.Text;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using Swashbuckle.AspNetCore.Filters;

namespace AITIssueTracker.Model.v0._4_Examples
{
    
    public class FeatureExample : IExamplesProvider<FeatureForm>
    {
        public FeatureForm GetExamples()
        {
            return new FeatureForm
            {
                Title = "Datenbank",
                Description = "Diskussion der Komponenten der angestrebten Datenbank.",
                Deadline = DateTime.MaxValue,
                StartDate = DateTime.Now,
                Status = FeatureStatus.ToDo
            };
        }
    }
}
