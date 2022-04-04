using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AITIssueTracker.API.v0._2_Manager;
using AITIssueTracker.API.v0._2_Manager.Contracts;
using AITIssueTracker.Model.v0;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Annotations;

namespace AITIssueTracker.API.v0._1_Controller
{
    [ApiController]
    [ApiVersion("0.0")]
    [Route(Endpoints.BASE_FEATURE)]
    [SwaggerTag("Manage project features and assignment of users this features.")]
    public class FeatureController : ControllerBase
    {
        private FeatureManager Manager { get; }
        public IFeatureService Service { get; }

        public FeatureController(FeatureManager manager, IFeatureService service)
        {
            Manager = manager;
            Service = service;
        }

        /// <summary>
        /// Returns all features from a given project.
        /// </summary>
        /// <param name="projectId"></param>
        ///
        /// 
        [HttpGet]
        [Route("{projectId:guid}")]
        public async Task<IActionResult> GetAllFeaturesOfProject(
            [FromRoute] Guid projectId)
        {
            if (!await Service.ProjectExistsAsync(projectId))
                return NotFound();
            
            List<FeatureView> featuresOfProject = await Service.GetFeaturesOfProjectAsync(projectId);

            if (featuresOfProject is null)
                return BadRequest();

            return Ok(featuresOfProject);
        }

        /// <summary>
        /// Create a new feature and assign it to a project.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="newFeature"></param>
        /// 
        [HttpPost]
        [Route("{projectId:guid}")]
        public async Task<IActionResult> PostNewFeatureAsync(
            [FromRoute] Guid projectId,
            [FromBody] FeatureForm newFeature)
        {
            FeatureView savedFeature = await Service.SaveFeatureToProjectAsync(projectId, newFeature);

            if (savedFeature is null)
                return BadRequest();

            return Ok(savedFeature);
        }
        /// <summary>
        /// Edits a features data.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="featureId"></param>
        /// <param name="newStatus"></param>
        /// 
        [HttpPut]
        [Route("{featureId:guid}/{newStatus}")]
        public async Task<IActionResult> PutFeatureStatusAsync(
            [FromQuery] string username,
            [FromRoute] Guid featureId,
            [FromRoute] FeatureStatus newStatus)
        {
            /*int res = await Manager.UpdateFeatureStatusAsync(featureId, newStatus, username);

            if (res == -1)
            {
                return BadRequest();
            }
            */

            return Ok();
        }

        /// <summary>
        /// Removes a feature from a project.
        /// </summary>
        /// <param name="featureId"></param>
        /// 
        [HttpDelete]
        [Route("{featureId:guid}")]
        public async Task<IActionResult> DeleteFeatureFromProjectAsync(
            [FromRoute] Guid featureId)
        {
            if (!await Manager.DeleteFeatureFromProjectAsync(featureId))
            {
                return BadRequest();
            }
            return Ok();
        }

        /* === User to Feature === */

        /// <summary>
        /// Adds a user to a feature.
        /// </summary>
        /// <param name="userToFeatureForm"></param>
        /// 
        [HttpPost]
        [Route("user")]
        public async Task<IActionResult> PostUserToFeatureAsync(
            [FromBody] FeatureUserForm userToFeatureForm)
        {

            if (!await Manager.AddUserToFeatureAsync(userToFeatureForm.FeatureId, userToFeatureForm.Username))
            {
                return BadRequest();
            }

            return Ok();
        }

        /// <summary>
        /// Remove a user from a feature.
        /// </summary>
        /// <param name="featureId"></param>
        /// <param name="username"></param>
        /// 
        [HttpDelete]
        [Route("user/{featureId:guid}/{username}")]
        public async Task<IActionResult> DeleteUserFromFeatureAsync(
            [FromRoute] Guid featureId,
            [FromRoute] string username)
        {

            if (!await Manager.RemoveUserFromFeatureAsync(featureId, username))
            {
                return BadRequest();
            }

            return Ok();
        }

    }
}
