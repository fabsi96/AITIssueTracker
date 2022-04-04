using System;
using System.Security.AccessControl;

namespace AITIssueTracker.Model.v0._2_EntityModel
{
    public class ProjectFeature
    {
        public Guid FeatureId { get; set; }

        public Guid ProjectId { get; set; }
    }
}