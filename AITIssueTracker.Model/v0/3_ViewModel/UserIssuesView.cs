using System.Collections.Generic;
using AITIssueTracker.Model.v0._2_EntityModel;

namespace AITIssueTracker.Model.v0._3_ViewModel
{
    public class UserIssuesView
    {
        public List<Issue> ProjectIssues { get; set; }

        public List<Issue> FeatureIssues { get; set; }
    }
}