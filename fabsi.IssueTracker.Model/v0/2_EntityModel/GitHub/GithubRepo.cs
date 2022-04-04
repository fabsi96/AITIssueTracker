using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AITIssueTracker.Model.v0._2_EntityModel.GitHub
{
    // Project (Missing: IsDone)
    public class GithubRepo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("updated_at")]
        public DateTime LastUpdate { get; set; }
    }

}
