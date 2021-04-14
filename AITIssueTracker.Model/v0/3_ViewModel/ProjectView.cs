using System;
using System.Collections.Generic;
using System.Text;

namespace AITIssueTracker.Model.v0._3_ViewModel
{
    public class ProjectView
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsDone { get; set; }
    }
}
