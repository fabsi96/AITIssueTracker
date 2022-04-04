using System.Collections.Generic;
using AITIssueTracker.Model.v0._2_EntityModel;
using Npgsql;

namespace AITIssueTracker.Model.v0._3_ViewModel
{
    public class FeatureIssuesView : Feature
    {
        public List<Issue> FeatureIssues { get; set; }

        public FeatureIssuesView() : base()
        {
            
        }
        
        public FeatureIssuesView(NpgsqlDataReader reader) : base(reader)
        {
            
        }
    }
}