using System.ComponentModel.DataAnnotations;

namespace AITIssueTracker.API.v0._1_Controller
{
    public class GithubInfoForm
    {
        [Required]
        public string PublicUsername { get; set; }

        [Required]
        public string RepoName { get; set; }
    }
}