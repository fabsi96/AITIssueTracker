using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AITIssueTracker.Model.v0;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace AITIssueTracker.API.v0._1_Controller
{
    [ApiController]
    [ApiVersion("0.0")]
    [Route(Endpoints.BASE_TEST)]
    [SwaggerTag("Test controller for testings/performance benchmarks")]
    public class TestController : ControllerBase
    {
        private DbTestContext DbContext { get; }
        
        public TestController(DbTestContext dbContext)
        {
            DbContext = dbContext;
        }
        
        /// <summary>
        /// Get all test objects
        /// </summary>
        ///
        [HttpGet]
        [ProducesResponseType(typeof(List<Test>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] string username)
        {
            var allItems = (await DbContext.Tests.ToListAsync())?.ConvertAll(c => c.AsView());

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
        [HttpPost]
        [ProducesResponseType(typeof(Test), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostNewItemAsync(
            [FromBody] TestForm newItem)
        {
            try
            {
                var savedItem = DbContext.Tests.Add(new Test(newItem)).Entity.AsView();
                int saveResult = await DbContext.SaveChangesAsync();
                return Ok(savedItem);
            }
            catch (Exception e)
            {
                return Conflict();
            }
        }
        
        [HttpPut]
        [Route("{testName}")]
        public async Task<IActionResult> PutEditItemAsync(
            [FromRoute] string testName,
            [FromBody] TestForm testData)
        {
            try
            {
                DbContext.Tests.Update(new Test(testData));
                DbContext.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }            
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
            try
            {
                Test targetEntity = DbContext.Tests.Single(t => t.Name.Equals(identifier));
                DbContext.Remove(targetEntity);
                int entries = await DbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }
    }
}
