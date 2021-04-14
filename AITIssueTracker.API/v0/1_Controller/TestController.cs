using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route(Endpoints.BASE_TEST)]
    [SwaggerTag("Test controller for testings/performance benchmarks")]
    public class TestController : ControllerBase
    {
        private TestManager Manager { get; }

        public TestController(TestManager manager)
        {
            Manager = manager;
        }

        /// <summary>
        /// Get all test objects
        /// </summary>
        ///
        ///
        [HttpGet]
        [ProducesResponseType(typeof(List<Test>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] string username)
        {
            List<TestView> allItems = await Manager.GetItemAsync();

            if (allItems is null)
            {
                return BadRequest();
            }

            return Ok(allItems);
        }

        /// <summary>
        /// Create a new test object
        /// </summary>
        ///
        ///
        [HttpPost]
        [ProducesResponseType(typeof(Test), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostNewItemAsync(
            [FromBody] TestForm newItem)
        {
            TestView savedItem = await Manager.SaveNewItemAsync(newItem);

            if (savedItem is null)
            {
                return BadRequest();
            }

            return Ok(savedItem);
        }

        /// <summary>
        /// Delete a test object with the target identifier
        /// </summary>
        ///
        /// 
        [HttpDelete]
        [Route("{identifier}")]
        [ProducesResponseType( 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteItemAsync(
            [FromRoute] string identifier)
        {
            if (!await Manager.DeleteItemByIdentifierAsync(identifier))
            {
                return BadRequest();
            }

            return Ok();
        }
    }

}
