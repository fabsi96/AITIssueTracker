using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NpgsqlTypes;

namespace AITIssueTracker.API.v0._1_Controller
{
    [ApiController]
    [ApiVersion("0.0")]
    [Route(Endpoints.BASE_FEATURE)]
    public class FeatureController : ControllerBase
    {
        private FeatureManager Manager { get; }

        public FeatureController(FeatureManager manager)
        {
            Manager = manager;
        }

        /// <summary>
        /// Returns all features from a given project.
        /// </summary>
        /// <param name="projectId"></param>
        ///
        /// 
        [HttpGet]
        [Route("{projectId}")]
        public async Task<IActionResult> GetAllFeaturesOfProject(
            [FromRoute] Guid projectId)
        {
            List<FeatureView> featuresOfProject = await Manager.GetFeaturesOfProjectAsync(projectId);

            if (featuresOfProject is null)
            {
                return BadRequest();
            }

            return Ok(featuresOfProject);
        }

        /// <summary>
        /// Create a new feature and assign it to a project.
        /// </summary>
        /// <param name="newFeature"></param>
        /// 
        [HttpPost]
        public async Task<IActionResult> PostNewFeatureAsync(
            [FromBody] FeatureForm newFeature)
        {
            FeatureView savedFeature = await Manager.SaveNewFeatureAsync(newFeature);

            if (savedFeature is null)
            {
                return BadRequest();
            }

            return Ok(savedFeature);
        }

        /// <summary>
        /// Removes a feature from a project.
        /// </summary>
        /// <param name="featureId"></param>
        /// 
        [HttpDelete]
        [Route("{featureId}")]
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
        [Route("user/{featureId}/{username}")]
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

    public class FeatureManager
    {
        private FeatureContext DbContext { get; }

        private ProjectContext ProjectDb { get; }

        public FeatureManager(FeatureContext ctx, 
            ProjectContext projectDb)
        {
            DbContext = ctx;
            ProjectDb = projectDb;
        }

        public async Task<FeatureView> SaveNewFeatureAsync(FeatureForm feature)
        {
            Feature featureToSave = new Feature(feature);
            return await DbContext.InsertNewFeatureAsync(featureToSave) == 1 ? featureToSave.AsView() : null;
        }

        public async Task<List<FeatureView>> GetFeaturesOfProjectAsync(Guid projectId)
        {
            List<Feature> projectFeatures = await DbContext.SelectFeaturesByProjectTitleAsync(projectId);
            return projectFeatures?.ConvertAll(c => c.AsView());
        }

        public async Task<bool> DeleteFeatureFromProjectAsync(Guid featureId)
        {
            return await DbContext.DeleteFeatureByIdentifier(featureId);
        }

        public async Task<bool> AddUserToFeatureAsync(Guid featureId, string username)
        {
            Project projectOfFeature = await DbContext.SelectProjectOfFeatureAsync(featureId);
            if (projectOfFeature is null)
                return false;

            bool userExists = await ProjectDb.SelectUserIsInProjectAsync(username, projectOfFeature.Id);

            if (!userExists)
                return false;

            return await DbContext.InsertUserToFeatureAsync(featureId, username) == 1;
        }

        public async Task<bool> RemoveUserFromFeatureAsync(Guid featureId, string username)
        {
            return await DbContext.DeleteUserFromFeatureAsync(featureId, username) == 1;
        }
    }

    public class FeatureContext
    {
        // === Basic ===
        private const string SQL_INSERT_NEW = "insert into \"feature\" (project_id, title, description, deadline, startdate, status) " +
                                              " values (@project_id, @title, @description, @deadline, @startdate, @status);";
        private const string SQL_SELECT_BY_PROJECT_TITLE = "select * from \"feature\" where project_id=@project_id;";
        private const string SQL_DELETE_BY_TITLE = "delete from \"feature\" where id=@id;";

        // === Extended ===
        private const string SQL_INSERT_USER_TO_FEATURE = "insert into \"feature_user\" (feature_id, username) values (@feature_id, @username);";
        private const string SQL_DELETE_USER_FROM_FEATURE = "delete from \"feature_user\" where feature_id=@feature_id and username=@username";
        private const string SQL_SELECT_PROJECT_BY_FEATURE = "select * from \"project\" as p join \"feature\" as f on p.id = f.project_id and f.id = @feature_id;";

        private string ConnectionString
        {
            get
            {
                return $"Server={Settings.DatabaseIp};Port={Settings.DatabasePort};User Id={Settings.Username};Password={Settings.Password};Database={Settings.Database};Pooling=false;";
            }
        }
        private PsqlSettings Settings { get; }

        public FeatureContext(PsqlSettings settings)
        {
            Settings = settings;
        }

        public async Task<int> InsertNewFeatureAsync(Feature featureToSave)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_INSERT_NEW;
                cmd.Parameters.Add("@title", NpgsqlDbType.Text).Value = featureToSave.Title;
                cmd.Parameters.Add("@description", NpgsqlDbType.Text).Value = featureToSave.Description;
                cmd.Parameters.Add("@deadline", NpgsqlDbType.Timestamp).Value = featureToSave.Deadline;
                cmd.Parameters.Add("@startdate", NpgsqlDbType.Timestamp).Value = featureToSave.StartDate;
                cmd.Parameters.Add("@status", NpgsqlDbType.Smallint).Value = (int)featureToSave.Status;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = featureToSave.ProjectId;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return res == 1 ? 1 : -1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"InsertNewFeatureAsync: Error: {e.Message}");
                return -1;
            }
        }

        public async Task<List<Feature>> SelectFeaturesByProjectTitleAsync(Guid projectId)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_SELECT_BY_PROJECT_TITLE;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                List<Feature> projectFeatures = new List<Feature>();
                while (await reader.ReadAsync())
                {
                    projectFeatures.Add(new Feature(reader));
                }
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return projectFeatures;
            }
            catch (Exception e)
            {
                Console.WriteLine($"SelectFeaturesByProjectTitleAsync: Error: {e.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteFeatureByIdentifier(Guid featureId)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_DELETE_BY_TITLE;
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = featureId;

                await cmd.PrepareAsync();
                int deleteResult = await cmd.ExecuteNonQueryAsync();
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return deleteResult != -1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"DeleteFeatureByIdentifier: Error: {e.Message}");
                return false;
            }
        }

        public async Task<int> InsertUserToFeatureAsync(Guid featureId, string username)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_INSERT_USER_TO_FEATURE;
                cmd.Parameters.Add("@feature_id", NpgsqlDbType.Uuid).Value = featureId;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return res != -1 ? 1 : -1;

            }
            catch (Exception e)
            {
                Console.WriteLine($"InsertUserToFeatureAsync: Error: {e.Message}");
                return -1;
            }
        }

        public async Task<int> DeleteUserFromFeatureAsync(Guid featureId, string username)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_DELETE_USER_FROM_FEATURE;
                cmd.Parameters.Add("@feature_id", NpgsqlDbType.Uuid).Value = featureId;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return res != -1 ? 1 : -1;

            }
            catch (Exception e)
            {
                Console.WriteLine($"DeleteUserFromFeatureAsync: Error: {e.Message}");
                return -1;
            }
        }

        public async Task<Project> SelectProjectOfFeatureAsync(Guid featureId)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_SELECT_PROJECT_BY_FEATURE;
                cmd.Parameters.Add("@feature_id", NpgsqlDbType.Uuid).Value = featureId;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                Project project = default;
                if (!await reader.ReadAsync())
                {
                    project = null;
                }
                else
                {
                    project = new Project(reader);
                }
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return project;

            }
            catch (Exception e)
            {
                Console.WriteLine($"SelectProjectOfFeatureAsync: Error: {e.Message}");
                return null;
            }
        }

    }
}
