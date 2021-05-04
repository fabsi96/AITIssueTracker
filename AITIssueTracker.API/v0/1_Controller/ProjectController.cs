using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0;
using AITIssueTracker.Model.v0._3_ViewModel;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;
using Swashbuckle.AspNetCore.Annotations;

namespace AITIssueTracker.API.v0._1_Controller
{
    [ApiController]
    [ApiVersion("0.0")]
    [Route(Endpoints.BASE_PROJECT)]
    [SwaggerTag("Manage projects and its developers")]
    public class ProjectController : ControllerBase
    {
        private IProjectService Service { get; }
        
        public ProjectController(IProjectService service)
        {
            Service = service;
        }

        /// <summary>
        /// Get all existing projects.
        /// </summary>
        /// <param name="filter"></param>
        /// 
        [HttpGet]
        public async Task<IActionResult> GetAllProjectsAsync(
            [FromQuery] string filter)
        {
            List<ProjectView> allProjects = await Service.GetAllProjectsAsync();
            return allProjects is null ? BadRequest() : Ok(allProjects);
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="newProject"></param>
        /// 
        [HttpPost]
        public async Task<IActionResult> PostNewProjectAsync(
            [FromBody] ProjectForm newProject)
        {
            if (Service.ProjectExists(newProject.Title))
            {
                return BadRequest("Already exists");
            }
            ProjectView savedProject = await Service.SaveProjectAsync(newProject);
            return savedProject is null ? BadRequest("Could not be saved") : Ok(savedProject);
        }

        /// <summary>
        /// Delete an existing project.
        /// </summary>
        /// <param name="projectId"></param>
        /// 
        [HttpDelete]
        [Route("{projectId:guid}")]
        public async Task<IActionResult> DeleteProjectAsync(
            [FromRoute] Guid projectId)
        {
            bool isDeleted = await Service.DeleteProjectAsync(projectId);

            return !isDeleted ? BadRequest("Could not be saved") : Ok("Successfully saved or is already removed");
        }

        /* === Extended User interactions === */

        /// <summary>
        /// Adds a user (developer) to a project.
        /// </summary>
        /// <param name="userToProjectForm"></param>
        /// 
        [HttpPost]
        [Route("user")]
        public async Task<IActionResult> PostUserToProjectAsync(
            [FromBody] ProjectUserForm userToProjectForm)
        {
            if (!await Service.AddUserToProjectAsync(userToProjectForm.Username, userToProjectForm.ProjectId))
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Removes a user (developer) from a project.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="username"></param>
        /// 
        [HttpDelete]
        [Route("user/{projectId:guid}/{username}")]
        public async Task<IActionResult> DeleteUserFromProjectAsync(
            [FromRoute] Guid projectId,
            [FromRoute] string username)
        {
            if (!await Service.RemoveUserFromProjectAsync(username, projectId))
            {
                return BadRequest();
            }

            return Ok();
        }
    }

    public interface IProjectService
    {
        Task<ProjectView> SaveProjectAsync(ProjectForm newProject);

        Task<List<ProjectView>> GetAllProjectsAsync();

        Task<bool> DeleteProjectAsync(Guid projectId);

        bool ProjectExists(string projectTitle);
        
        Task<bool> AddUserToProjectAsync(string username, Guid projectId);
        
        Task<bool> RemoveUserFromProjectAsync(string username, Guid projectId);
    }

    public class ProjectManager : IProjectService
    {
        private ProjectContext DbContext { get; }

        public ProjectManager(ProjectContext ctx)
        {
            DbContext = ctx;
        }

        public async Task<ProjectView> SaveProjectAsync(ProjectForm projectForm)
        {
            Project newProject = new Project(projectForm);
            return await DbContext.InsertNewAsync(newProject) == 1 ? newProject.AsView() : null;
        }

        public async Task<List<ProjectView>> GetAllProjectsAsync()
        {
            List<Project> allProjects = await DbContext.SelectAllAsync();
            return allProjects?.ConvertAll(c => c.AsView());
        }

        public async Task<bool> DeleteProjectAsync(Guid projectId)
        {
            return await DbContext.DeleteByTitleAsync(projectId);
        }

        public bool ProjectExists(string projectTitle)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddUserToProjectAsync(string username, Guid projectId)
        {
            return await DbContext.InsertUserToProjectAsync(username, projectId) == 1;
        }

        public async Task<bool> RemoveUserFromProjectAsync(string username, Guid projectId)
        {
            return await DbContext.DeleteUserFromProjectAsync(username, projectId) == 1;
        }
    }

    public class ProjectContext
    {
        private const string SQL_INSERT_NEW = "insert into \"project\" (title, description, is_done) values (@title, @description, @is_done);";
        private const string SQL_SELECT_ALL = "select id, title, description, is_done from \"project\";";
        private const string SQL_DELETE_BY_TITLE = "delete from \"project\" where id=@id;";

        private string ConnectionString
        {
            get
            {
                return $"Server={Settings.DatabaseIp};Port={Settings.DatabasePort};User Id={Settings.Username};Password={Settings.Password};Database={Settings.Database};Pooling=false;";
            }
        }
        private PsqlSettings Settings { get; }

        public ProjectContext(PsqlSettings settings)
        {
            Settings = settings;
        }

        public async Task<int> InsertNewAsync(Project newProject)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_INSERT_NEW;
                cmd.Parameters.Add("@title", NpgsqlDbType.Text).Value = newProject.Title;
                cmd.Parameters.Add("@description", NpgsqlDbType.Text).Value = newProject.Description;
                cmd.Parameters.Add("@is_done", NpgsqlDbType.Boolean).Value = newProject.IsDone;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return res != 1 ? -1 : 1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"InsertNewAsync: Error: {e.Message}");
                return -1;
            }
        }

        public async Task<List<Project>> SelectAllAsync()
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_SELECT_ALL;

                await cmd.PrepareAsync();
                List<Project> projects = new List<Project>();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    projects.Add(new Project(reader));
                }

                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return projects;
            }
            catch (Exception e)
            {
                Console.WriteLine($"SelectAllAsync: Error: {e.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteByTitleAsync(Guid id)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_DELETE_BY_TITLE;
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = id;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return res != -1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"DeleteByTitleAsync: Error: {e.Message}");
                return false;
            }
        }

        public async Task<int> InsertUserToProjectAsync(string username, Guid projectId)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                string SQL_INSERT_USER_TO_PROJECT = "insert into \"project_user\" (project_id, username) values (@project_id, @username);";
                cmd.CommandText = SQL_INSERT_USER_TO_PROJECT;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;
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
                Console.WriteLine($"InsertUserToProjectAsync: Error: {e.Message}");
                return -1;
            }
        }

        public async Task<int> DeleteUserFromProjectAsync(string username, Guid projectId)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                string SQL_DELETE_USER_FROM_PROJECT = "delete from \"project_user\" where project_id=@project_id and username=@username;";
                cmd.CommandText = SQL_DELETE_USER_FROM_PROJECT;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;
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
                Console.WriteLine($"DeleteUserFromProjectAsync: Error: {e.Message}");
                return -1;
            }
        }

        public async Task<bool> SelectUserIsInProjectAsync(string username, Guid projectId)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                string SQL_USER_EXISTS_IN_PROJECT = "select * from \"project_user\" where username=@username and project_id = @project_id;";
                cmd.CommandText = SQL_USER_EXISTS_IN_PROJECT;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                bool userExists = reader.HasRows;
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return userExists;
            }
            catch (Exception e)
            {
                Console.WriteLine($"SelectUserIsInProjectAsync: Error: {e.Message}");
                return false;
            }
        }
    }
}
