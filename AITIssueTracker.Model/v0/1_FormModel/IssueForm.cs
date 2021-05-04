using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using AITIssueTracker.Model.v0._2_EntityModel;

namespace AITIssueTracker.Model.v0._1_FormModel
{
    public class IssueForm
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public IssueType Type { get; set; }

        public int EffortEstimation { get; set; }

        [Required]
        public FeatureStatus Status { get; set; }
    }
}
