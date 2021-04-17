using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using AITIssueTracker.Model.v0._2_EntityModel;

namespace AITIssueTracker.Model.v0._1_FormModel
{
    public class FeatureForm
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime Deadline { get; set; }

        public DateTime StartDate { get; set; }

        [Required]
        public FeatureStatus Status { get; set; }
    }
}
