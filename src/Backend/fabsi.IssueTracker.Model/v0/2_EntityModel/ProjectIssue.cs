using System;

namespace AITIssueTracker.Model.v0._2_EntityModel
{
    public class ProjectIssue
    {
        public Guid IssueId { get; set; }

        public Guid ProjectId { get; set; }
    }
}