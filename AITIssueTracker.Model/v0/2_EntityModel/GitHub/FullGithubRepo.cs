using System;
using System.Collections.Generic;
using System.Text;

namespace AITIssueTracker.Model.v0._2_EntityModel.GitHub
{
    public class FullGithubRepo
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime LastUpdate { get; set; }

        public List<GithubRepoIssue> Issues { get; set; }

        public List<GithubMilestone> Features { get; set; }
    }

}
