using System.Collections.Generic;
using AITIssueTracker.Model.v0._2_EntityModel;

namespace AITIssueTracker.Model.v0._3_ViewModel
{
    public class IssuesView
    {
        public List<Issue> ProjectIssues { get; set; }

        public List<FeatureIssuesView> ProjectFeatures { get; set; }
    }
}