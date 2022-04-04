using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AITIssueTracker.Model.v0._2_EntityModel.GitHub
{
    // Feature (Missing: None)
    public class GithubMilestone
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("state")]
        public string Status { get; set; }

        [JsonProperty("updated_at")]
        public DateTime LastUpdate { get; set; }
    }
}
