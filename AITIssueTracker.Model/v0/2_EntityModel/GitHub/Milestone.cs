using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AITIssueTracker.Model.v0._2_EntityModel.GitHub
{
    // Nested milestone
    public class Milestone
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
