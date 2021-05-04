using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._2_Manager;
using AITIssueTracker.Model.v0;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AITIssueTracker.API.v0._1_Controller
{
    [ApiController]
    [ApiVersion("0.0")]
    [Route(Endpoints.BASE_ISSUE)]
    [SwaggerTag("Manage issues. Project issues and feature issues and asign users to them.")]
    public class IssueController : ControllerBase
    {
        private IssueManager Manager { get; }

        public IssueController(IssueManager manager)
        {
            Manager = manager;
        }

        /// <summary>
        /// Returns all issues of a project assigned to the project
        /// </summary>
        /// <param name="projectId"></param>
        /// 
        [HttpGet]
        [Route("project/{projectId}")]
        public async Task<IActionResult> GetIssuesOfProjectAsync(
        [FromRoute] Guid projectId)
        {
            List<IssueView> issuesOfProject = await Manager.GetIssuesOfProjectAsync(projectId);

            if (issuesOfProject is null)
            {
                return BadRequest();
            }

            return Ok(issuesOfProject);
        }

        /// <summary>
        /// Assign a new issue to a project.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="newIssue"></param>
        /// 
        [HttpPost]
        [Route("project/{projectId}")]
        public async Task<IActionResult> PostNewIssueToProjectAsync(
            [FromRoute] Guid projectId,
            [FromBody] IssueForm newIssue)
        {
            IssueView issue = await Manager.SaveProjectIssueAsync(newIssue, projectId);

            if (issue is null)
            {
                return BadRequest();
            }

            return Ok(issue);
        }

        /// <summary>
        /// Update the type of an issue.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="issueId"></param>
        /// <param name="issueType"></param>
        /// 
        [HttpPut]
        [Route("type/{issueId}/{issueType}")]
        public async Task<IActionResult> PutIssueTypeUpdateAsync(
            [FromQuery] string username,
            [FromRoute] Guid issueId,
            [FromRoute] IssueType issueType)
        {
            IssueView updatedIssue = await Manager.EditIssueTypeAsync(issueType, issueId, username);

            if (updatedIssue is null)
            {
                return BadRequest();
            }

            return Ok(updatedIssue);
        }

        /// <summary>
        /// Update the status of an issue.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="issueId"></param>
        /// <param name="issueStatus"></param>
        /// 
        [HttpPut]
        [Route("status/{issueId}/{issueStatus}")]
        public async Task<IActionResult> PutIssueStatusUpdateAsync(
            [FromQuery] string username,
            [FromRoute] Guid issueId,
            [FromRoute] FeatureStatus issueStatus)
        {
            IssueView updatedIssue = await Manager.EditIssueStatusAsync(issueStatus, issueId, username);

            if (updatedIssue is null)
            {
                return BadRequest();
            }

            return Ok(updatedIssue);
        }
        /// <summary>
        /// Delete an issue from a project.
        /// </summary>
        /// <param name="issueId"></param>
        /// 
        [HttpDelete]
        [Route("project/{issueId}")]
        public async Task<IActionResult> DeleteIssueFromProjectAsync(
            [FromRoute] Guid issueId)
        {
            if (!await Manager.DeleteIssueFromProjectAsync(issueId))
            {
                return BadRequest();
            }

            return Ok();
        }

        /// <summary>
        /// Returns all issues assign to the given feature.
        /// </summary>
        /// <param name="featureId"></param>
        /// 
        [HttpGet]
        [Route("feature/{featureId}")]
        public async Task<IActionResult> GetIssuesOfFeatureAsync(
            [FromRoute] Guid featureId)
        {
            List<IssueView> issuesOfFeature = await Manager.GetIssuesOfFeatureAsync(featureId);

            if (issuesOfFeature is null)
            {
                return BadRequest();
            }

            return Ok(issuesOfFeature);
        }

        /// <summary>
        /// Assign a new issue to a feature of a project.
        /// </summary>
        /// <param name="featureId"></param>
        /// <param name="newIssue"></param>
        /// 
        [HttpPost]
        [Route("feature/{featureId}")]
        public async Task<IActionResult> PostNewIssueToFeatureAsync(
            [FromRoute] Guid featureId,
            [FromBody] IssueForm newIssue)
        {
            IssueView issue = await Manager.SaveFeatureIssueAsync(newIssue, featureId);

            if (issue is null)
            {
                return BadRequest();
            }

            return Ok(issue);
        }

        /// <summary>
        /// Removes an issue from a feature.
        /// </summary>
        /// <param name="issueId"></param>
        /// 
        [HttpDelete]
        [Route("feature/{issueId}")]
        public async Task<IActionResult> DeleteIssueFromFeatureAsync(
            [FromRoute] Guid issueId)
        {
            if (!await Manager.DeleteIssueFromFeatureAsync(issueId))
            {
                return BadRequest();
            }

            return Ok();
        }

        /* === Extended user to issue functions === */

        /// <summary>
        /// Assing a user to an issue of a project.
        /// </summary>
        /// <param name="userToIssueForm"></param>
        /// 
        [HttpPost]
        [Route("user")]
        public async Task<IActionResult> PostUserToProjectIssueAsync(
            [FromBody] UserIssueForm userToIssueForm)
        {
            if (!await Manager.AddUserToIssueAsync(userToIssueForm.IssueId, userToIssueForm.Username))
            {
                return BadRequest();
            }

            return Ok();
        }

        /// <summary>
        /// Removes a user from a project issue.
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="username"></param>
        /// 
        [HttpDelete]
        [Route("project/user/{issueId}/{username}")]
        public async Task<IActionResult> DeleteUserFromProjectIssueAsync(
            [FromRoute] Guid issueId,
            [FromRoute] string username)
        {

            if (!await Manager.RemoveUserFromProjectIssueAsync(issueId, username))
            {
                return BadRequest();
            }

            return Ok();
        }

        /// <summary>
        /// Removes the user from a feature.
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="username"></param>
        /// 
        [HttpDelete]
        [Route("feature/user/{issueId}/{username}")]
        public async Task<IActionResult> DeleteUserFromFeatureIssueAsync(
            [FromRoute] Guid issueId,
            [FromRoute] string username)
        {

            if (!await Manager.RemoveUserFromFeatureIssueAsync(issueId, username))
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
