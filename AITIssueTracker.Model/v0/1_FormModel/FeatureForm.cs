using System;
using System.Collections.Generic;
using System.Text;
using AITIssueTracker.Model.v0._2_EntityModel;

namespace AITIssueTracker.Model.v0._1_FormModel
{
    public class FeatureForm
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public Guid ProjectId { get; set; }

        public DateTime Deadline { get; set; }

        public DateTime StartDate { get; set; }

        public FeatureStatus Status { get; set; }
    }
}
