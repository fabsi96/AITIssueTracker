using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AITIssueTracker.Model.v0._1_FormModel
{
    public class ProjectForm
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsDone { get; set; } = false;
    }
}
