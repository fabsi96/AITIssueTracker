using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._2_EntityModel.GitHub;
using Npgsql;
using NpgsqlTypes;

namespace AITIssueTracker.API.v0._1_Controller
{
    public class GithubContext : PsqlMaster
    {
        private const string HTTP_GITHUB_GET_REPOSITORIES = "/users/{0}/repos";
        private const string HTTP_GITHUB_GET_REPOSITORY_INFO = "repos/{0}/{1}";
        private const string HTTP_GITHUB_GET_REPOSITORY_MILESTONES = "/repos/{0}/{1}/milestones";
        private const string HTTP_GITHUB_GET_REPOSITORY_ISSUES = "/repos/{0}/{1}/issues";

        public GithubContext(PsqlSettings dbSettings) : base(dbSettings)
        {
            
        }

        public async Task<FullGithubRepo> FetchGithubInfosAsync(string username, string repoName)
        {
            try
            {
                FullGithubRepo repoInfos = new FullGithubRepo();
                GithubRepo repo = await GetApiContentAsJsonAsync<GithubRepo>(
                    string.Format(
                        HTTP_GITHUB_GET_REPOSITORY_INFO, username, repoName)
                );
                repoInfos.Id = repo.Id;
                repoInfos.Title = repo.Title;
                repoInfos.Description = repo.Description;
                repoInfos.LastUpdate = repo.LastUpdate;

                List<GithubRepoIssue> issues = await GetApiContentAsJsonAsync<List<GithubRepoIssue>>(
                    string.Format(HTTP_GITHUB_GET_REPOSITORY_ISSUES, username, repoName)
                );
                repoInfos.Issues = issues;

                List<GithubMilestone> features = await GetApiContentAsJsonAsync<List<GithubMilestone>>(
                    string.Format(HTTP_GITHUB_GET_REPOSITORY_MILESTONES, username, repoName)
                );
                repoInfos.Features = features;

                return repoInfos;
            }
            catch (Exception e)
            {
                Console.WriteLine($"FetchGithubInfosAsync: Error: {e.Message}");
                return null;
            }
        }

        private HttpClient client;
        async Task<T> GetApiContentAsJsonAsync<T>(string urlWithParams)
        {
            try
            {
                client = new HttpClient
                {
                    BaseAddress = new Uri("https://api.github.com")
                };
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("product", "1"));

                HttpResponseMessage response = await client.GetAsync(urlWithParams);
                return await response.Content.ReadAsAsync<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetApiContentAsync: Error: {e.Message}");
                return default;
            }
        }

        private const string SQL_INSERT_PROJECT = "insert into \"project\" (id, title, description, is_done) values (@id, @title, @description, @is_done);";
        private const string SQL_INSERT_FEATURE = "insert into \"feature\" (id, project_id, title, description, deadline, startdate, status) " +
                                                  " values (@id, @project_id, @title, @description, @deadline, @startdate, @status);";
        private const string SQL_INSERT_ISSUE = "insert into \"issue\" " +
                                                " (id, title, description, issue_type, effort_estimation, status) " +
                                                " values " +
                                                " (@id, @title, @description, @issue_type, @effort_estimation, @status);";
        private const string SQL_INSERT_PROJECT_ISSUE = "insert into \"project_issue\" (issue_id, project_id) values (@issue_id, @project_id);";
        private const string SQL_INSERT_FEATURE_ISSUE = "insert into \"feature_issue\" (issue_id, feature_id) values (@issue_id, @feature_id);";

        public async Task<bool> InsertFullGithubProjectAsync(FullGithubRepo fullRepoInformations)
        {
            bool isSucceeded = false;

            isSucceeded = await ExecuteSqlAsync(async (cmd) =>
            {
                int dbRes = 0;
                NpgsqlTransaction transaction = await cmd.Connection.BeginTransactionAsync();
                cmd.Transaction = transaction;

                // === Insert project ===
                Project project = new Project
                {
                    Title = fullRepoInformations.Title,
                    Description = fullRepoInformations.Description,
                    IsDone = false,
                };
                cmd.CommandText = SQL_INSERT_PROJECT;
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = project.Id;
                cmd.Parameters.Add("@title", NpgsqlDbType.Text).Value = project.Title;
                cmd.Parameters.Add("@description", NpgsqlDbType.Text).Value = project.Description;
                cmd.Parameters.Add("@is_done", NpgsqlDbType.Boolean).Value = project.IsDone;

                await cmd.PrepareAsync();
                dbRes = await cmd.ExecuteNonQueryAsync();

                if (dbRes != 1)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                // === CleanUp ===
                cmd.Parameters.Clear();

                // === Insert features to project ===
                Dictionary<int, Feature> features = new Dictionary<int, Feature>();
                foreach (GithubMilestone githubMilestone in fullRepoInformations.Features)
                {
                    Feature feature = new Feature
                    {
                        Title = githubMilestone.Title,
                        Description = githubMilestone.Description,
                        Status = ConvertStatusToFeature(githubMilestone.Status),
                    };
                    features.Add(githubMilestone.Id, feature);

                    cmd.CommandText = SQL_INSERT_FEATURE;
                    cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = feature.Id;
                    cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = project.Id;
                    cmd.Parameters.Add("@title", NpgsqlDbType.Text).Value = feature.Title;
                    cmd.Parameters.Add("@description", NpgsqlDbType.Text).Value = feature.Description;
                    cmd.Parameters.Add("@deadline", NpgsqlDbType.Timestamp).Value = feature.Deadline;
                    cmd.Parameters.Add("@startdate", NpgsqlDbType.Timestamp).Value = feature.StartDate;
                    cmd.Parameters.Add("@status", NpgsqlDbType.Smallint).Value = (int)feature.Status;

                    await cmd.PrepareAsync();
                    dbRes = await cmd.ExecuteNonQueryAsync();

                    if (dbRes != 1)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }

                    cmd.Parameters.Clear();
                }

                // === Insert issues to project ===
                foreach (GithubRepoIssue githubRepoIssue in fullRepoInformations.Issues)
                {
                    Issue issue = new Issue
                    {
                        Title = githubRepoIssue.Title,
                        Description = githubRepoIssue.Description,
                        EffortEstimation = 0,
                        Status = ConvertStatusToFeature(githubRepoIssue.Labels.FirstOrDefault()?.Name),
                        Type = ConvertLabelToType(githubRepoIssue.Labels.FirstOrDefault()?.Name),
                    };

                    cmd.CommandText = SQL_INSERT_ISSUE;
                    cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = issue.Id;
                    cmd.Parameters.Add("@title", NpgsqlDbType.Text).Value = issue.Title;
                    cmd.Parameters.Add("@description", NpgsqlDbType.Text).Value = issue.Description;
                    cmd.Parameters.Add("@issue_type", NpgsqlDbType.Varchar).Value = issue.Type.ToString();
                    cmd.Parameters.Add("@effort_estimation", NpgsqlDbType.Integer).Value = issue.EffortEstimation;
                    cmd.Parameters.Add("@status", NpgsqlDbType.Varchar).Value = issue.Status.ToString();

                    await cmd.PrepareAsync();
                    dbRes = await cmd.ExecuteNonQueryAsync();

                    if (dbRes != 1)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                    cmd.Parameters.Clear();

                    if (githubRepoIssue.Milestone != null)
                    {
                        // Assign issue to feature
                        Feature featureOfIssue = features[githubRepoIssue.Milestone.Id];
                        cmd.CommandText = SQL_INSERT_FEATURE_ISSUE;
                        cmd.Parameters.Add("@issue_id", NpgsqlDbType.Uuid).Value = issue.Id;
                        cmd.Parameters.Add("@feature_id", NpgsqlDbType.Uuid).Value = featureOfIssue.Id;

                        await cmd.PrepareAsync();
                        dbRes = await cmd.ExecuteNonQueryAsync();

                        if (dbRes != 1)
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }
                    }
                    else
                    {
                        // Assign issue to project
                        cmd.CommandText = SQL_INSERT_PROJECT_ISSUE;
                        cmd.Parameters.Add("@issue_id", NpgsqlDbType.Uuid).Value = issue.Id;
                        cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = project.Id;

                        await cmd.PrepareAsync();
                        dbRes = await cmd.ExecuteNonQueryAsync();

                        if (dbRes != 1)
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }
                    }

                    cmd.Parameters.Clear();
                }

                await transaction.CommitAsync();
                return true;
            }, false);

            return isSucceeded;
        }

        private FeatureStatus ConvertStatusToFeature(string githubStatusCode)
        {
            switch (githubStatusCode)
            {
                case "To do":
                    return FeatureStatus.ToDo;

                default:
                    return FeatureStatus.ToDo;
            }
        }

        private IssueType ConvertLabelToType(string label)
        {
            switch (label)
            {
                default:
                    return IssueType.Improvement;
            }
        }

        public async Task<GithubProject> SelectSyncProjectAsync(int id)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                GithubProject project = default;
                cmd.CommandText = "select * from \"project_sync\" where github_repo_id=@repo_id;";
                cmd.Parameters.Add("?", NpgsqlDbType.Integer).Value = id;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (!reader.HasRows)
                    return null;

                await reader.ReadAsync();
                return new GithubProject(reader);

            }, null);
        }

        public async Task<bool> UpdateGithubProjectAsync(GithubProject latestSync, FullGithubRepo fullRepoInformations)
        {
            throw new NotImplementedException();
        }
    }
}