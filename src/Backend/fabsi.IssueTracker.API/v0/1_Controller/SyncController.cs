using System.Threading.Tasks;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel.GitHub;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AITIssueTracker.API.v0._1_Controller
{
    [ApiController]
    [ApiVersion("0.0")]
    [SwaggerTag("Synch between an existing github repo (project) and the issuetracker")]
    [Route("api/sync")]
    public class SyncController : ControllerBase
    {
        private GithubContext DbContext { get; set; }

        public SyncController(GithubContext ctx)
        {
            DbContext = ctx;
        }

        [HttpPost]
        public async Task<IActionResult> SyncWithGithubAsync(
            // [FromQuery] string currentUsername, // TODO: How to transfer the github username to username in the own system?
            [FromBody] GithubInfoForm githubInfoForm)
        {
            FullGithubRepo fullRepoInformations = await DbContext.FetchGithubInfosAsync(
                    githubInfoForm.PublicUsername, 
                    githubInfoForm.RepoName
                );

            if (fullRepoInformations is null)
            {
                return BadRequest();
            }

            GithubProject latestSync = await DbContext.SelectSyncProjectAsync(fullRepoInformations.Id);

            if (latestSync is null)
            {
                // New project - try to sync.
                bool insertSuccess = await DbContext.InsertFullGithubProjectAsync(fullRepoInformations);
                if (!insertSuccess)
                {
                    return BadRequest();
                }
            }
            else
            {
                // Filter data
                bool insertSuccess = await DbContext.UpdateGithubProjectAsync(latestSync, fullRepoInformations);
                if (!insertSuccess)
                {
                    return BadRequest();
                }
            }

            return Ok();
        }
    }
}
