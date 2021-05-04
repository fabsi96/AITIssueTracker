using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._2_Manager;
using AITIssueTracker.Model.v0;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AITIssueTracker.API.v0._1_Controller
{
    [ApiController]
    [ApiVersion("0.0")]
    [Route(Endpoints.BASE_VIEW)]
    [SwaggerTag("Use-Cases")]
    public class ViewController : ControllerBase
    {
        private ViewManager Manager { get; }
        public ViewController(ViewManager manager)
        {
            Manager = manager;
        }

        /// <summary>
        /// Anwendungsfall 1
        /// </summary>
        /// 
        [HttpGet]
        [Route("project")]
        public async Task<IActionResult> GetAllProjectsAsync()
        {
            List<ProjectView> allProjects = await Manager.GetAllProjectsAsync();

            if (allProjects is null)
            {
                return BadRequest();
            }

            return Ok(allProjects);
        }

        /// <summary>
        /// Anwendungsfall 2
        /// </summary>
        /// 
        [HttpGet]
        [Route("project/issues/{projectId}")]
        public async Task<IActionResult> GetAllIssuesOfProjectAsync(
            [FromRoute] Guid projectId)
        {
            IssuesView allIssuesView = await Manager.GetAllFeaturesAndIssuesOfProjectAsync(projectId);

            if (allIssuesView is null)
            {
                return BadRequest();
            }

            return Ok(allIssuesView);
        }

        /// <summary>
        /// Anwendungsfall 3
        /// </summary>
        /// 
        [HttpGet]
        [Route("project/{username}")]
        public async Task<IActionResult> GetFeaturesAndIssuesOfUserAsync(
            [FromRoute] string username)
        {
            IssuesView userIssuesView = await Manager.GetAllFeaturesAndIssuesOfUserAsync(username);

            if (userIssuesView is null)
                return BadRequest();

            return Ok(userIssuesView);
        }

        /// <summary>
        /// Anwendungsfall 4
        /// </summary>
        /// 
        [HttpGet]
        [Route("project/user/{projectId}")]
        public async Task<IActionResult> GetUsersOfProjectAsync(
            [FromRoute] Guid projectId)
        {
            List<UserView> usersOfProject = await Manager.GetUsersOfProjectAsync(projectId);

            if (usersOfProject is null)
                return BadRequest();

            return Ok(usersOfProject);
        }

        /// <summary>
        /// Anwendungsfall 5
        /// </summary>
        /// 
        [HttpGet]
        [Route("user/issue/{username}")]
        public async Task<IActionResult> GetAllIssuesOfUserAsync(
            [FromRoute] string username)
        {
            UserIssuesView allIssuesViewOfUser = await Manager.GetIssuesOfUserAsync(username);

            if (allIssuesViewOfUser is null)
                return BadRequest();

            return Ok(allIssuesViewOfUser);
        }
    }
}
