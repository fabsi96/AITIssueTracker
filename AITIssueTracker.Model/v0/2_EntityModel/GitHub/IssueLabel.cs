using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AITIssueTracker.Model.v0._2_EntityModel.GitHub
{
    public class IssueLabel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

}
