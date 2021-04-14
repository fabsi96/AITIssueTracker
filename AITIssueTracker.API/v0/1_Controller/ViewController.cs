using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;

namespace AITIssueTracker.API.v0._1_Controller
{
    [ApiController]
    [Route(Endpoints.BASE_VIEW)]
    [ApiVersion("0.0")]
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
            Issues allIssues = await Manager.GetAllFeaturesAndIssuesOfProjectAsync(projectId);

            if (allIssues is null)
            {
                return BadRequest();
            }

            return Ok(allIssues);
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
            Issues userIssues = await Manager.GetAllFeaturesAndIssuesOfUserAsync(username);

            if (userIssues is null)
                return BadRequest();

            return Ok(userIssues);
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
            UserIssues allIssuesOfUser = await Manager.GetIssuesOfUserAsync(username);

            if (allIssuesOfUser is null)
                return BadRequest();

            return Ok(allIssuesOfUser);
        }
    }

    public class ViewManager
    {
        private ViewContext DbContext { get; }

        public ViewManager(ViewContext ctx)
        {
            DbContext = ctx;
        }

        public async Task<List<ProjectView>> GetAllProjectsAsync()
        {
            List<Project> allProjects = await DbContext.SelectAllProjectsAsync();
            return allProjects?.ConvertAll(c => c.AsView());
        }

        public async Task<Issues> GetAllFeaturesAndIssuesOfProjectAsync(Guid projectId)
        {
            return await DbContext.SelectIssuesOfProjectAsync(projectId);
        }

        public async Task<Issues> GetAllFeaturesAndIssuesOfUserAsync(string username)
        {
            return await DbContext.SelectIssuesOfUserAsync(username);
        }

        public async Task<List<UserView>> GetUsersOfProjectAsync(Guid projectId)
        {
            List<User> allUsers = await DbContext.SelectUsersOfProjectAsync(projectId);
            return allUsers?.ConvertAll(c => c.AsView());
        }

        public async Task<UserIssues> GetIssuesOfUserAsync(string username)
        {
            Issues issuesOfUser = await DbContext.SelectIssuesOfUserAsync(username);
            List<FeatureIssue> usersFeatureIssues = new List<FeatureIssue>();
            issuesOfUser.ProjectFeatures.ForEach(issue =>
            {
                usersFeatureIssues.AddRange(issue.FeatureIssues);
            });

            UserIssues userIssues = new UserIssues
            {
                ProjectIssues = issuesOfUser.ProjectIssues,
                FeatureIssues = usersFeatureIssues,
            };

            return userIssues;
        }
    }

    public class ViewContext
    {
        private string ConnectionString
        {
            get
            {
                return $"Server={Settings.DatabaseIp};Port={Settings.DatabasePort};User Id={Settings.Username};Password={Settings.Password};Database={Settings.Database};Pooling=false;";
            }
        }
        private PsqlSettings Settings { get; }
        public ViewContext(PsqlSettings settings)
        {
            Settings = settings;
        }

        public async Task<List<Project>> SelectAllProjectsAsync()
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                string SQL_SELECT_ALL_PROJECTS = "select * from \"project\";";
                cmd.CommandText = SQL_SELECT_ALL_PROJECTS;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                List<Project> projects = new List<Project>();
                while (await reader.ReadAsync())
                {
                    projects.Add(new Project(reader));
                }

                // CleanUp
                await reader.DisposeAsync();
                await cmd.DisposeAsync();
                await conn.CloseAsync();
                await conn.DisposeAsync();

                return projects;

            }
            catch (Exception e)
            {
                Console.WriteLine($"SelectAllProjectsAsync: Error: {e.Message}");
                return null;
            }
        }

        public async Task<Issues> SelectIssuesOfProjectAsync(Guid projectId)
        {
            try
            {
                Issues allIssues = new Issues
                {
                    ProjectIssues = new List<ProjectIssue>(),
                    ProjectFeatures = new List<FeatureWithIssues>()
                };

                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                string SQL_SELECT_PROJECT_ISSUES = "select * from \"issue_project\" where project_id=@project_id;";
                cmd.CommandText = SQL_SELECT_PROJECT_ISSUES;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;

                /* === Issues of project === */
                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    allIssues.ProjectIssues.Add(new ProjectIssue(reader));
                }

                // CleanUp
                cmd.Parameters.Clear();
                await reader.DisposeAsync();
                
                /* === Features of Project === */
                string SQL_SELECT_PROJECT_FEATURES = "select * from \"feature\" where project_id = @project_id;";
                cmd.CommandText = SQL_SELECT_PROJECT_FEATURES;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;

                await cmd.PrepareAsync();
                reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    allIssues.ProjectFeatures.Add(new FeatureWithIssues(reader));
                }

                // CleanUp
                cmd.Parameters.Clear();
                await reader.DisposeAsync();

                /* === Issues of each feature === */
                foreach (FeatureWithIssues feature in allIssues.ProjectFeatures)
                {
                    feature.FeatureIssues = new List<FeatureIssue>();
                    string SQL_SELECT_FEATURE_ISSUES = "select * from \"issue_feature\" where feature_id = @feature_id;";
                    cmd.CommandText = SQL_SELECT_FEATURE_ISSUES;
                    cmd.Parameters.Add("@feature_id", NpgsqlDbType.Uuid).Value = feature.Id;

                    await cmd.PrepareAsync();
                    reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        feature.FeatureIssues.Add(new FeatureIssue(reader));
                    }

                    // CleanUp
                    cmd.Parameters.Clear();
                    await reader.DisposeAsync();
                }



                // CleanUp
                await reader.DisposeAsync();
                await cmd.DisposeAsync();
                await conn.CloseAsync();
                await conn.DisposeAsync();

                return allIssues;
            }
            catch (Exception e)
            {
                Console.WriteLine($"SelectIssuesOfProjectAsync: Error: {e.Message}");
                return null;
            }
        }

        public async Task<Issues> SelectIssuesOfUserAsync(string username)
        {

            try
            {
                Issues userIssues = new Issues
                {
                    ProjectIssues = new List<ProjectIssue>(),
                    ProjectFeatures = new List<FeatureWithIssues>()
                };

                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();

                /* === Project issues === */
                string SQL_SELECT_USER_PROJECT_ISSUES = "select * from \"issue_project\" as ip join \"issue_project_user\" as ipu on ip.id=ipu.issue_project_id where ipu.username=@username;";
                cmd.CommandText = SQL_SELECT_USER_PROJECT_ISSUES;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;


                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    userIssues.ProjectIssues.Add(new ProjectIssue(reader));
                }

                // CleanUp
                cmd.Parameters.Clear();
                await reader.DisposeAsync();

                /* === Features and its issues === */
                string SQL_SELECT_USER_FEATURES = "select * from \"feature\" as f join \"feature_user\" as fu on f.id = fu.feature_id where fu.username=@username;";
                cmd.CommandText = SQL_SELECT_USER_FEATURES;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;

                await cmd.PrepareAsync();
                reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    userIssues.ProjectFeatures.Add(new FeatureWithIssues(reader));
                }

                // CleanUp
                cmd.Parameters.Clear();
                await reader.DisposeAsync();

                /* === .. issues === */
                foreach (FeatureWithIssues feature in userIssues.ProjectFeatures)
                {
                    feature.FeatureIssues = new List<FeatureIssue>();
                    string SQL_FEATURE_ISSUES = "select * from \"issue_feature\" as issf join \"issue_feature_user\" as issfu on " +
                                                "   issf.id = issfu.issue_feature_id " +
                                                " and " +
                                                "   issf.feature_id = @feature_id " +
                                                " and " +
                                                "   issfu.username=@username;";

                    cmd.CommandText = SQL_FEATURE_ISSUES;
                    cmd.Parameters.Add("@feature_id", NpgsqlDbType.Uuid).Value = feature.Id;
                    cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;

                    await cmd.PrepareAsync();
                    reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        feature.FeatureIssues.Add(new FeatureIssue(reader));
                    }

                    // CleanUp
                    cmd.Parameters.Clear();
                    await reader.DisposeAsync();
                }

                // CleanUp
                await reader.DisposeAsync();
                await cmd.DisposeAsync();
                await conn.CloseAsync();
                await conn.DisposeAsync();

                return userIssues;
            }
            catch (Exception e)
            {
                Console.WriteLine($"SelectIssuesOfUserAsync: Error: {e.Message}");
                return null;
            }
        }

        public async Task<List<User>> SelectUsersOfProjectAsync(Guid projectId)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                string SQL_SELECT_USERS_OF_PROJECT = "select * from \"user\" as u join \"project_user\" as pu on u.username = pu.username and pu.project_id = @project_id;";
                cmd.CommandText = SQL_SELECT_USERS_OF_PROJECT;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;

                await cmd.PrepareAsync();
                List<User> users = new List<User>();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    users.Add(new User(reader));
                }

                // CleanUp
                await reader.DisposeAsync();
                await cmd.DisposeAsync();
                await conn.CloseAsync();
                await conn.DisposeAsync();

                return users;

            }
            catch (Exception e)
            {
                Console.WriteLine($"SelectUsersOfProjectAsync: Error: {e.Message}");
                return null;
            }
        }
    }

    public class Issues
    {
        public List<ProjectIssue> ProjectIssues { get; set; }

        public List<FeatureWithIssues> ProjectFeatures { get; set; }
    }

    public class FeatureWithIssues : Feature
    {
        public List<FeatureIssue> FeatureIssues { get; set; }

        public FeatureWithIssues() : base()
        {
            
        }
        
        public FeatureWithIssues(NpgsqlDataReader reader) : base(reader)
        {
            
        }
    }

    public class UserIssues
    {
        public List<ProjectIssue> ProjectIssues { get; set; }

        public List<FeatureIssue> FeatureIssues { get; set; }
    }
}
