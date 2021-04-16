using System;
using System.Collections.Generic;
using System.Text;
using AITIssueTracker.Model.v0._2_EntityModel;

namespace AITIssueTracker.Model.v0._1_FormModel
{
    public class IssueForm
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public IssueType Type { get; set; }

        public int EffortEstimation { get; set; }

        public FeatureStatus Status { get; set; }
    }
}
