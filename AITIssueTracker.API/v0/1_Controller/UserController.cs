using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._2_Manager;
using AITIssueTracker.Model.v0;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AITIssueTracker.API.v0._1_Controller
{
    [ApiController]
    [ApiVersion("0.0")]
    [Route(Endpoints.BASE_USER)]
    [SwaggerTag("Manages Users. Basic operations like GET, POST and DELETE")]
    public class UserController : ControllerBase
    {
        private UserManager Manager { get; }

        public UserController(UserManager manager)
        {
            Manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            List<UserView> allUsers = await Manager.GetUsersAsync();

            if (allUsers is null)
            {
                return BadRequest();
            }

            return Ok(allUsers);
        }

        [HttpPost]
        public async Task<IActionResult> PostNewUserAsync(
            [FromBody] UserForm newUser)
        {
            UserView savedUser = await Manager.SaveNewUserAsync(newUser);

            if (savedUser is null)
            {
                return BadRequest();
            }

            return Ok(savedUser);
        }

        [HttpDelete]
        [Route("{username}")]
        public async Task<IActionResult> DeleteUserByName(
            [FromRoute] string username)
        {
            if (!await Manager.DeleteUserByUsername(username))
            {
                return BadRequest();
            }
            return Ok();
        }

        /* === === */
    }
}
