using System;
using System.Linq;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._2_Manager;
using AITIssueTracker.API.v0._2_Manager.Contracts;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AITIssueTracker.API.v0._1_Controller
{
    [ApiController]
    [ApiVersion("0.0")]
    [Route(Endpoints.BASE_USER)]
    [SwaggerTag(Endpoints.User.SWAGGER_TAG)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            try
            {
                return Ok(await _service.GetAllUsersAsync());
            }
            catch (Exception e)
            {
                // Some internal db stuff (like db does not exist)
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostNewUserAsync(
            [FromBody] UserForm newUser)
        {
            if (_service.UserExists(newUser.Username))
                return StatusCode(StatusCodes.Status409Conflict, new ErrorInfo("PostNewUserAsync: User already exists."));
            
            try
            {
                return Ok(await _service.SaveUserAsync(newUser));
            }
            catch (Exception e)
            {
                // Any internal database error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        [Route(Endpoints.User.USER_BY_USERNAME)]
        public async Task<IActionResult> DeleteUserByName(
            [FromRoute] string username)
        {
            if (!_service.UserExists(username))
                return NotFound(new ErrorInfo($"DeleteUserByName: User not found."));

            try
            {
                await _service.DeleteUserByUsernameAsync(username);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
