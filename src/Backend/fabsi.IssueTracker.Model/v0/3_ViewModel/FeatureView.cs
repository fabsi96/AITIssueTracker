using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using AITIssueTracker.Model.v0._2_EntityModel;

namespace AITIssueTracker.Model.v0._3_ViewModel
{
    public class FeatureView
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime Deadline { get; set; }

        public DateTime StartDate { get; set; }

        public FeatureStatus Status { get; set; }
    }
}
