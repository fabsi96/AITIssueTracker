using System;
using System.Collections.Generic;
using System.Text;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using Swashbuckle.AspNetCore.Filters;

namespace AITIssueTracker.Model.v0._4_Examples
{
    public class IssueExample : IExamplesProvider<IssueForm>
    {
        public IssueForm GetExamples()
        {
            return new IssueForm
            {
                Title = "Modellierung des DAL",
                Description = "Definierung der Daten im DAL. Einschließlich Datentypen, Normalformen, ...",
                Type = IssueType.Task,
                EffortEstimation = 12345,
                Status = FeatureStatus.ToDo,
            };
        }
    }
}
