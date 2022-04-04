using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.Model.v0._2_EntityModel;
using Npgsql;
using NpgsqlTypes;

namespace AITIssueTracker.API.v0._3_DAL
{
    public class IssueContext : PsqlMaster
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

        private const string SQL_UPDATE_ISSUE = "update \"issue\" set title=@title, description=@description, effort_estimation=@effort_estimation, issue_type=@issue_type, status=@status where id=@id;";

        private const string SQL_DELETE_ISSUE_BY_ID = "delete from \"issue\" where id=@id;";
        private const string SQL_DELETE_ISSUE_FROM_PROJECT_BY_ID = "delete from \"project_issue\" where issue_id=@issue_id;";
        private const string SQL_DELETE_ISSUE_FROM_FEATURE_BY_ID = "delete from \"feature_issue\" where issue_id=@issue_id;";

        // === Extended ===
        private const string SQL_SELECT_PROJECT_OF_PROJECT_ISSUE = "select * from \"project\" as p join \"project_issue\" as pi on p.id = pi.project_id and pi.issue_id = @issue_id;";
        private const string SQL_SELECT_PROJECT_OF_FEATURE_ISSUE = "select * from \"project\" as p join \"feature\" as f on p.id = f.project_id join \"feature_issue\" as fi on f.id = fi.feature_id and fi.issue_id=@issue_id;";

        private const string SQL_INSERT_USER_TO_ISSUE = "insert into \"issue_user\" (issue_id, username) values (@issue_id, @username);";

        private const string SQL_DELETE_USER_FROM_ISSUE = "delete from \"issue_user\" where issue_id=@issue_id and username=@username;";

        public IssueContext(PsqlSettings settings) : base(settings)
        {
        }

        public async Task<List<Issue>> SelectIssuesOfProjectAsync(Guid projectId)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {

                cmd.CommandText = SQL_SELECT_PROJECT_ISSUES;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                List<Issue> issues = new List<Issue>();
                while (await reader.ReadAsync())
                {
                    issues.Add(new Issue(reader));
                }
                return issues;
            }, null);
        }

        public async Task<List<Issue>> SelectIssuesOfFeatureAsync(Guid featureId)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_SELECT_FEATURE_ISSUES;
                cmd.Parameters.Add("@feature_id", NpgsqlDbType.Uuid).Value = featureId;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                List<Issue> issues = new List<Issue>();
                while (await reader.ReadAsync())
                {
                    issues.Add(new Issue(reader));
                }
                return issues;

            }, null);
        }

        public async Task<int> InsertNewProjectIssueAsync(Issue issue, Guid projectId)
        {
            // return await ExecuteSqlAsync(async (cmd) => { }, -1);
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
            // return await ExecuteSqlAsync(async (cmd) => { }, -1);
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

        public async Task<int> UpdateIssueAsync(Issue issue)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_UPDATE_ISSUE;
                cmd.Parameters.Add("@title", NpgsqlDbType.Text).Value = issue.Title;
                cmd.Parameters.Add("@description", NpgsqlDbType.Text).Value = issue.Description;
                cmd.Parameters.Add("@issue_type", NpgsqlDbType.Varchar).Value = issue.Type.ToString();
                cmd.Parameters.Add("@effort_estimation", NpgsqlDbType.Integer).Value = issue.EffortEstimation;
                cmd.Parameters.Add("@status", NpgsqlDbType.Varchar).Value = issue.Status.ToString();
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = issue.Id;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                return res != -1 ? 1 : -1;

            }, -1);
        }

        public async Task<int> DeleteIssueFromProjectById(Guid issueId)
        {
            // return await ExecuteSqlAsync(async (cmd) => { }, -1);
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
            // return await ExecuteSqlAsync(async (cmd) => { }, -1);
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
            return await ExecuteSqlAsync(async (cmd) =>
            {
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
                return project;

            }, null);
        }

        public async Task<int> InsertUserToIssueAsync(Guid issueId, string username)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_INSERT_USER_TO_ISSUE;

                cmd.Parameters.Add("@issue_id", NpgsqlDbType.Uuid).Value = issueId;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                return res != -1 ? 1 : -1;

            }, -1);
        }

        public async Task<int> DeleteUserFromIssueAsync(Guid issueId, string username)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_DELETE_USER_FROM_ISSUE;

                cmd.Parameters.Add("@issue_id", NpgsqlDbType.Uuid).Value = issueId;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                return res != -1 ? 1 : -1;

            }, -1);
        }

        public async Task<Issue> SelectIssueAsync(Guid issueId)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                string SQL_SELECT_ISSUE_BY_ID = "select * from \"issue\" where id=@id;";
                cmd.CommandText = SQL_SELECT_ISSUE_BY_ID;
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = issueId;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                Issue targetIssue = default;
                if (await reader.ReadAsync())
                    targetIssue = new Issue(reader);
                else
                    targetIssue = null;
                return targetIssue;

            }, null);
        }
    }
}