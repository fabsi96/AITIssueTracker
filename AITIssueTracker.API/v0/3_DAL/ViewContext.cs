using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Npgsql;
using NpgsqlTypes;

namespace AITIssueTracker.API.v0._3_DAL
{
    public class ViewContext : PsqlMaster
    {
        private const string SQL_SELECT_PROJECT_ISSUES = "select * from \"issue\" as i join \"project_issue\" as pi on i.id=pi.issue_id and pi.project_id=@project_id;";
        private const string SQL_SELECT_ALL_PROJECTS = "select * from \"project\";";
        private const string SQL_SELECT_USERS_OF_PROJECT = "select * from \"user\" as u join \"project_user\" as pu on u.username = pu.username and pu.project_id = @project_id;";
        private const string SQL_SELECT_PROJECT_FEATURES = "select * from \"feature\" where project_id=@project_id;";
        private const string SQL_SELECT_FEATURE_ISSUES = "select * from \"issue\" as i join \"feature_issue\" as fi on i.id = fi.issue_id where fi.feature_id=@feature_id;";
        private const string SQL_SELECT_USER_PROJECT_ISSUES = "select * from \"issue\" as i join \"project_issue\" as pi on i.id = pi.issue_id join \"issue_user\" as iu on i.id = iu.issue_id and iu.username=@username;";
        private const string SQL_SELECT_USER_FEATURES = "select * from \"feature\" as f join \"feature_user\" as fu on f.id = fu.feature_id where fu.username=@username;";

        private const string SQL_FEATURE_ISSUES = "select * from \"issue\" as i join \"feature_issue\" as fi on " +
                                                  "   i.id = fi.issue_id join \"issue_user\" as iu on i.id = iu.issue_id " +
                                                  " and " +
                                                  "   fi.feature_id = @feature_id " +
                                                  " and " +
                                                  "   iu.username = @username; ";
        public ViewContext(PsqlSettings settings) : base(settings)
        {
        }

        public async Task<List<Project>> SelectAllProjectsAsync()
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_SELECT_ALL_PROJECTS;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                List<Project> projects = new List<Project>();
                while (await reader.ReadAsync())
                {
                    projects.Add(new Project(reader));
                }

                return projects;
            }, null);
        }

        public async Task<IssuesView> SelectIssuesOfProjectAsync(Guid projectId)
        {
            // return await ExecuteSqlAsync(async (cmd) => { }, -1);
            try
            {
                IssuesView allIssuesView = new IssuesView
                {
                    ProjectIssues = new List<Issue>(),
                    ProjectFeatures = new List<FeatureIssuesView>()
                };

                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_SELECT_PROJECT_ISSUES;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;

                /* === IssuesView of project === */
                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    allIssuesView.ProjectIssues.Add(new Issue(reader));
                }

                // CleanUp
                cmd.Parameters.Clear();
                await reader.DisposeAsync();
                
                /* === Features of Project === */
                cmd.CommandText = SQL_SELECT_PROJECT_FEATURES;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;

                await cmd.PrepareAsync();
                reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    allIssuesView.ProjectFeatures.Add(new FeatureIssuesView(reader));
                }

                // CleanUp
                cmd.Parameters.Clear();
                await reader.DisposeAsync();

                /* === IssuesView of each feature === */
                foreach (FeatureIssuesView feature in allIssuesView.ProjectFeatures)
                {
                    feature.FeatureIssues = new List<Issue>();
                    cmd.CommandText = SQL_SELECT_FEATURE_ISSUES;
                    cmd.Parameters.Add("@feature_id", NpgsqlDbType.Uuid).Value = feature.Id;

                    await cmd.PrepareAsync();
                    reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        feature.FeatureIssues.Add(new Issue(reader));
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

                return allIssuesView;
            }
            catch (Exception e)
            {
                Console.WriteLine($"SelectIssuesOfProjectAsync: Error: {e.Message}");
                return null;
            }
        }

        public async Task<IssuesView> SelectIssuesOfUserAsync(string username)
        {
            // return await ExecuteSqlAsync(async (cmd) => { }, -1);
            try
            {
                IssuesView userIssuesView = new IssuesView
                {
                    ProjectIssues = new List<Issue>(),
                    ProjectFeatures = new List<FeatureIssuesView>()
                };

                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();

                /* === Project issues === */
                cmd.CommandText = SQL_SELECT_USER_PROJECT_ISSUES;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    userIssuesView.ProjectIssues.Add(new Issue(reader));
                }

                // CleanUp
                cmd.Parameters.Clear();
                await reader.DisposeAsync();

                /* === Features and its issues === */
                cmd.CommandText = SQL_SELECT_USER_FEATURES;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;

                await cmd.PrepareAsync();
                reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    userIssuesView.ProjectFeatures.Add(new FeatureIssuesView(reader));
                }

                // CleanUp
                cmd.Parameters.Clear();
                await reader.DisposeAsync();

                /* === .. issues === */
                foreach (FeatureIssuesView feature in userIssuesView.ProjectFeatures)
                {
                    feature.FeatureIssues = new List<Issue>();

                    cmd.CommandText = SQL_FEATURE_ISSUES;
                    cmd.Parameters.Add("@feature_id", NpgsqlDbType.Uuid).Value = feature.Id;
                    cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;

                    await cmd.PrepareAsync();
                    reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        feature.FeatureIssues.Add(new Issue(reader));
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

                return userIssuesView;
            }
            catch (Exception e)
            {
                Console.WriteLine($"SelectIssuesOfUserAsync: Error: {e.Message}");
                return null;
            }
        }

        public async Task<List<User>> SelectUsersOfProjectAsync(Guid projectId)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_SELECT_USERS_OF_PROJECT;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;

                await cmd.PrepareAsync();
                List<User> users = new List<User>();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    users.Add(new User(reader));
                }


                return users;
            }, null);
        }
    }
}