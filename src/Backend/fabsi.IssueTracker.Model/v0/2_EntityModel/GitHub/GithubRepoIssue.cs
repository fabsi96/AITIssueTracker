using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AITIssueTracker.Model.v0._2_EntityModel.GitHub
{
    // Project issue (Missing: Type)
    public class GithubRepoIssue
    {
        [JsonProperty("milestone")]
        public Milestone Milestone { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Description { get; set; }

        [JsonProperty("labels")]
        public List<IssueLabel> Labels { get; set; }

        [JsonProperty("updated_at")]
        public DateTime LastUpdate { get; set; }
    }

}
