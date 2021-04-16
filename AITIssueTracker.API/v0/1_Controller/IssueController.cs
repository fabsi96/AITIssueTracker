using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;

namespace AITIssueTracker.API.v0._1_Controller
{
    [ApiController]
    [ApiVersion("0.0")]
    [Route(Endpoints.BASE_ISSUE)]
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
        /// Assign a new issue to a project.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="newProjectIssue"></param>
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
        /// Assign a new issue to a feature of a project.
        /// </summary>
        /// <param name="newFeatureIssue"></param>
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
        /// <param name="userToProjectIssueForm"></param>
        /// 
        [HttpPost]
        [Route("project/user")]
        public async Task<IActionResult> PostUserToProjectIssueAsync(
            [FromBody] UserProjectIssueForm userToProjectIssueForm)
        {
            if (!await Manager.AddUserToProjectIssueAsync(userToProjectIssueForm.IssueId, userToProjectIssueForm.Username))
            {
                return BadRequest();
            }

            return Ok();
        }

        /// <summary>
        /// Assign a user to an issue of a feature of a project.
        /// </summary>
        /// <param name="userToFeatureIssueForm"></param>
        /// 
        [HttpPost]
        [Route("feature/user")]
        public async Task<IActionResult> PostUserToFeatureIssueAsync(
            [FromBody] UserFeatureIssueForm userToFeatureIssueForm)
        {

            if (!await Manager.AddUserToFeatureIssueAsync(userToFeatureIssueForm.IssueId, userToFeatureIssueForm.Username))
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

    public class IssueManager
    {
        private IssueContext DbContext { get; }
        private ProjectContext ProjectDb { get; }

        public IssueManager(IssueContext ctx, ProjectContext projectDb)
        {
            DbContext = ctx;
            ProjectDb = projectDb;
        }

        public async Task<List<IssueView>> GetIssuesOfProjectAsync(Guid projectId)
        {
            List<Issue> issues = await DbContext.SelectIssuesOfProjectAsync(projectId);
            return issues?.ConvertAll(c => c.AsView());
        }

        public async Task<List<IssueView>> GetIssuesOfFeatureAsync(Guid featureId)
        {
            List<Issue> issues = await DbContext.SelectIssuesOfFeatureAsync(featureId);
            return issues?.ConvertAll(c => c.AsView());
        }

        public async Task<IssueView> SaveProjectIssueAsync(IssueForm issue, Guid projectId)
        {
            Issue issueToSave = new Issue(issue);
            return await DbContext.InsertNewProjectIssueAsync(issueToSave, projectId) == 1 ? issueToSave.AsView() : null;
        }

        public async Task<IssueView> SaveFeatureIssueAsync(IssueForm issue, Guid featureId)
        {
            Issue newIssue = new Issue(issue);
            return await DbContext.InsertNewFeatureIssueAsync(newIssue, featureId) == 1 ? newIssue.AsView() : null;
        }

        public async Task<bool> DeleteIssueFromProjectAsync(Guid issueId)
        {
            return await DbContext.DeleteIssueFromProjectById(issueId) == 1;
        }

        public async Task<bool> DeleteIssueFromFeatureAsync(Guid issueId)
        {
            return await DbContext.DeleteIssueFromFeatureById(issueId) == 1;
        }

        public async Task<bool> AddUserToProjectIssueAsync(Guid issueId, string username)
        {
            Project project = await DbContext.SelectProjectOfIssueAsync(IssueContext.PROJECT_ISSUE, issueId, username);

            if (project is null)
                return false;

            bool userExists = await ProjectDb.SelectUserIsInProjectAsync(username, project.Id);

            if (!userExists)
                return false;

            return await DbContext.InsertUserToIssueAsync(issueId, username) == 1;
        }

        public async Task<bool> AddUserToFeatureIssueAsync(Guid issueId, string username)
        {
            Project project = await DbContext.SelectProjectOfIssueAsync(IssueContext.FEATURE_ISSUE, issueId, username);

            if (project is null)
                return false;

            bool userExists = await ProjectDb.SelectUserIsInProjectAsync(username, project.Id);

            if (!userExists)
                return false;

            return await DbContext.InsertUserToIssueAsync(issueId, username) == 1;
        }

        public async Task<bool> RemoveUserFromProjectIssueAsync(Guid issueId, string username)
        {
            return await DbContext.DeleteUserFromIssueAsync(issueId, username) == 1;
        }

        public async Task<bool> RemoveUserFromFeatureIssueAsync(Guid issueId, string username)
        {
            return await DbContext.DeleteUserFromIssueAsync(issueId, username) == 1;
        }
    }

    public class IssueContext
    {
        public const string PROJECT_ISSUE = "project-issue";
        public const string FEATURE_ISSUE = "feature-issue";

        /* === SQL Statements === */

        // === Basic ===
        private const string SQL_SELECT_PROJECT_ISSUES = "select * from \"issue\" as i join \"project_issue\" as pi on i.id = pi.issue_id and pi.project_id=@project_id;";
        private const string SQL_SELECT_FEATURE_ISSUES = "select * from \"issue\" as i join \"feature_issue\" as fi on i.id=fi.issue_id and fi.feature_id=@feature_id;";

        private const string SQL_INSERT_ISSUE = "insert into \"issue\" " +
                                                  " (id, title, description, issue_type, effort_estimation, status) " +
                                                  " values " +
                                                  " (@id, @title, @description, @issue_type, @effort_estimation, @status);";
        private const string SQL_INSERT_PROJECT_ISSUE = "insert into \"project_issue\" (issue_id, project_id) values (@issue_id, @project_id);";
        private const string SQL_INSERT_FEATURE_ISSUE = "insert into \"feature_issue\" (issue_id, feature_id) values (@issue_id, @feature_id);";

        private const string SQL_DELETE_ISSUE_BY_ID = "delete from \"issue\" where id=@id;";
        private const string SQL_DELETE_ISSUE_FROM_PROJECT_BY_ID = "delete from \"project_issue\" where issue_id=@issue_id;";
        private const string SQL_DELETE_ISSUE_FROM_FEATURE_BY_ID = "delete from \"feature_issue\" where issue_id=@issue_id;";

        // === Extended ===
        private const string SQL_SELECT_PROJECT_OF_PROJECT_ISSUE = "select * from \"project\" as p join \"project_issue\" as pi on p.id = pi.project_id and pi.issue_id = @issue_id;";
        private const string SQL_SELECT_PROJECT_OF_FEATURE_ISSUE = "select * from \"project\" as p join \"feature\" as f on p.id = f.project_id join \"feature_issue\" as fi on f.id = fi.feature_id and fi.issue_id=@issue_id;";

        private const string SQL_INSERT_USER_TO_ISSUE = "insert into \"issue_user\" (issue_id, username) values (@issue_id, @username);";

        private const string SQL_DELETE_USER_FROM_ISSUE = "delete from \"issue_user\" where issue_id=@issue_id and username=@username;";

        private string ConnectionString
        {
            get
            {
                return $"Server={Settings.DatabaseIp};Port={Settings.DatabasePort};User Id={Settings.Username};Password={Settings.Password};Database={Settings.Database};Pooling=false;";
            }
        }

        private PsqlSettings Settings { get; }

        public IssueContext(PsqlSettings settings)
        {
            Settings = settings;
        }

        public async Task<List<Issue>> SelectIssuesOfProjectAsync(Guid projectId)
        {

            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_SELECT_PROJECT_ISSUES;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                List<Issue> issues = new List<Issue>();
                while (await reader.ReadAsync())
                {
                    issues.Add(new Issue(reader));
                }
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return issues;

            }
            catch (Exception e)
            {
                Console.WriteLine($"SelectIssuesOfProjectAsync: Error: {e.Message}");
                return null;
            }
        }

        public async Task<List<Issue>> SelectIssuesOfFeatureAsync(Guid featureId)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_SELECT_FEATURE_ISSUES;
                cmd.Parameters.Add("@feature_id", NpgsqlDbType.Uuid).Value = featureId;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                List<Issue> issues = new List<Issue>();
                while (await reader.ReadAsync())
                {
                    issues.Add(new Issue(reader));
                }
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return issues;

            }
            catch (Exception e)
            {
                Console.WriteLine($"SelectIssuesOfFeatureAsync: Error: {e.Message}");
                return null;
            }
        }

        public async Task<int> InsertNewProjectIssueAsync(Issue issue, Guid projectId)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = await conn.BeginTransactionAsync();

                cmd.CommandText = SQL_INSERT_ISSUE;
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = issue.Id;
                cmd.Parameters.Add("@title", NpgsqlDbType.Text).Value = issue.Title;
                cmd.Parameters.Add("@description", NpgsqlDbType.Text).Value = issue.Description;
                cmd.Parameters.Add("@issue_type", NpgsqlDbType.Varchar).Value = issue.Type.ToString();
                cmd.Parameters.Add("@effort_estimation", NpgsqlDbType.Integer).Value = issue.EffortEstimation;
                cmd.Parameters.Add("@status", NpgsqlDbType.Varchar).Value = issue.Status.ToString();
                
                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                if (res != 1)
                {
                    // Error
                    await cmd.Transaction.RollbackAsync();

                    // CleanUp
                    await cmd.DisposeAsync();
                    await conn.CloseAsync();
                    await conn.DisposeAsync();

                    return -1;
                }

                // CleanUp
                cmd.Parameters.Clear();

                cmd.CommandText = SQL_INSERT_PROJECT_ISSUE;
                cmd.Parameters.Add("@issue_id", NpgsqlDbType.Uuid).Value = issue.Id;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;

                await cmd.PrepareAsync();
                res = await cmd.ExecuteNonQueryAsync();

                if (res != 1)
                {
                    // Error
                    await cmd.Transaction.RollbackAsync();

                    // CleanUp
                    await cmd.DisposeAsync();
                    await conn.CloseAsync();
                    await conn.DisposeAsync();

                    return -1;
                }

                await cmd.Transaction.CommitAsync();

                // CleanUp
                await cmd.DisposeAsync();
                await conn.CloseAsync();
                await conn.DisposeAsync();

                return 1;

            }
            catch (Exception e)
            {
                Console.WriteLine($"InsertNewProjectIssueAsync: Error: {e.Message}");
                return -1;
            }
        }

        public async Task<int> InsertNewFeatureIssueAsync(Issue issue, Guid featureId)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = await conn.BeginTransactionAsync();

                cmd.CommandText = SQL_INSERT_ISSUE;
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = issue.Id;
                cmd.Parameters.Add("@title", NpgsqlDbType.Text).Value = issue.Title;
                cmd.Parameters.Add("@description", NpgsqlDbType.Text).Value = issue.Description;
                cmd.Parameters.Add("@issue_type", NpgsqlDbType.Varchar).Value = issue.Type.ToString();
                cmd.Parameters.Add("@effort_estimation", NpgsqlDbType.Integer).Value = issue.EffortEstimation;
                cmd.Parameters.Add("@status", NpgsqlDbType.Varchar).Value = issue.Status.ToString();

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                if (res != 1)
                {
                    // Error
                    await cmd.Transaction.RollbackAsync();

                    // CleanUp
                    await cmd.DisposeAsync();
                    await conn.CloseAsync();
                    await conn.DisposeAsync();

                    return -1;
                }

                // CleanUp
                cmd.Parameters.Clear();

                cmd.CommandText = SQL_INSERT_FEATURE_ISSUE;
                cmd.Parameters.Add("@issue_id", NpgsqlDbType.Uuid).Value = issue.Id;
                cmd.Parameters.Add("@feature_id", NpgsqlDbType.Uuid).Value = featureId;

                await cmd.PrepareAsync();
                res = await cmd.ExecuteNonQueryAsync();

                if (res != 1)
                {
                    // Error
                    await cmd.Transaction.RollbackAsync();

                    // CleanUp
                    await cmd.DisposeAsync();
                    await conn.CloseAsync();
                    await conn.DisposeAsync();

                    return -1;
                }

                await cmd.Transaction.CommitAsync();

                // CleanUp
                await cmd.DisposeAsync();
                await conn.CloseAsync();
                await conn.DisposeAsync();

                return 1;

            }
            catch (Exception e)
            {
                Console.WriteLine($"InsertNewFeatureIssueAsync: Error: {e.Message}");
                return -1;
            }
        }

        public async Task<int> DeleteIssueFromProjectById(Guid issueId)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = await conn.BeginTransactionAsync();

                cmd.CommandText = SQL_DELETE_ISSUE_FROM_PROJECT_BY_ID;
                cmd.Parameters.Add("@issue_id", NpgsqlDbType.Uuid).Value = issueId;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                if (res != 1)
                {
                    // Error
                    await cmd.Transaction.RollbackAsync();

                    // CleanUp
                    await cmd.DisposeAsync();
                    await conn.CloseAsync();
                    await conn.DisposeAsync();

                    return -1;
                }

                // CleanUp
                cmd.Parameters.Clear();

                cmd.CommandText = SQL_DELETE_ISSUE_BY_ID;
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = issueId;

                await cmd.PrepareAsync();
                res = await cmd.ExecuteNonQueryAsync();
                if (res != 1)
                {
                    // Error
                    await cmd.Transaction.RollbackAsync();

                    // CleanUp
                    await cmd.DisposeAsync();
                    await conn.CloseAsync();
                    await conn.DisposeAsync();

                    return -1;
                }

                await cmd.Transaction.CommitAsync();

                // CleanUp
                await cmd.DisposeAsync();
                await conn.CloseAsync();
                await conn.DisposeAsync();

                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"DeleteIssueFromProjectById: Error: {e.Message}");
                return -1;
            }
        }

        public async Task<int> DeleteIssueFromFeatureById(Guid issueId)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = await conn.BeginTransactionAsync();

                cmd.CommandText = SQL_DELETE_ISSUE_FROM_FEATURE_BY_ID;
                cmd.Parameters.Add("@issue_id", NpgsqlDbType.Uuid).Value = issueId;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                if (res != 1)
                {
                    // Error
                    await cmd.Transaction.RollbackAsync();

                    // CleanUp
                    await cmd.DisposeAsync();
                    await conn.CloseAsync();
                    await conn.DisposeAsync();

                    return -1;
                }

                // CleanUp
                cmd.Parameters.Clear();

                cmd.CommandText = SQL_DELETE_ISSUE_BY_ID;
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = issueId;

                await cmd.PrepareAsync();
                res = await cmd.ExecuteNonQueryAsync();
                if (res != 1)
                {
                    // Error
                    await cmd.Transaction.RollbackAsync();

                    // CleanUp
                    await cmd.DisposeAsync();
                    await conn.CloseAsync();
                    await conn.DisposeAsync();

                    return -1;
                }

                await cmd.Transaction.CommitAsync();

                // CleanUp
                await cmd.DisposeAsync();
                await conn.CloseAsync();
                await conn.DisposeAsync();

                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"DeleteIssueFromFeatureById: Error: {e.Message}");
                return -1;
            }
        }

        public async Task<Project> SelectProjectOfIssueAsync(string ISSUE_TYPE, Guid issueId, string username)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = ISSUE_TYPE == PROJECT_ISSUE
                    ? SQL_SELECT_PROJECT_OF_PROJECT_ISSUE
                    : SQL_SELECT_PROJECT_OF_FEATURE_ISSUE;
                
                cmd.Parameters.Add("@issue_id", NpgsqlDbType.Uuid).Value = issueId;

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
                Console.WriteLine($"SelectProjectOfIssueAsync: Error: {e.Message}");
                return null;
            }
        }

        public async Task<int> InsertUserToIssueAsync(Guid issueId, string username)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_INSERT_USER_TO_ISSUE;

                cmd.Parameters.Add("@issue_id", NpgsqlDbType.Uuid).Value = issueId;
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
                Console.WriteLine($"InsertUserToIssueAsync: Error: {e.Message}");
                return -1;
            }
        }

        public async Task<int> DeleteUserFromIssueAsync(Guid issueId, string username)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_DELETE_USER_FROM_ISSUE;

                cmd.Parameters.Add("@issue_id", NpgsqlDbType.Uuid).Value = issueId;
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
                Console.WriteLine($"DeleteUserFromIssueAsync: Error: {e.Message}");
                return -1;
            }
        }
    }
}
