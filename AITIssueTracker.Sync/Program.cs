using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AITIssueTracker.Model.v0._2_EntityModel.GitHub;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AITIssueTracker.Sync
{
    class Program
    {
        private const string HTTP_GITHUB_GET_REPOSITORIES = "/users/{0}/repos";
        private const string HTTP_GITHUB_GET_REPOSITORY_INFO = "repos/{0}/{1}";
        private const string HTTP_GITHUB_GET_REPOSITORY_MILESTONES = "/repos/{0}/{1}/milestones";
        private const string HTTP_GITHUB_GET_REPOSITORY_ISSUES = "/repos/{0}/{1}/issues";

        private static HttpClient client;
        static async Task Main(string[] args)
        {
            FullGithubRepo repo = await ReadProjectFromGithubAsync();
            Console.ReadKey();
        }

        static async Task<FullGithubRepo> ReadProjectFromGithubAsync()
        {
            try
            {
                Console.WriteLine("=== GitHub Synchronisierung ===");
                string publicUsername = "fabsi96";
                string projectName = "GithubTest"; // string.Empty;
                Console.Write($"Benutzername    :");
                publicUsername = Console.ReadLine();
                Console.Write($"Projektname     :");
                projectName = Console.ReadLine();

                FullGithubRepo repoInfos = new FullGithubRepo();
                GithubRepo repo = await GetGithubAsync<GithubRepo>(
                    string.Format(
                        HTTP_GITHUB_GET_REPOSITORY_INFO, publicUsername, projectName)
                    );
                repoInfos.Id = repo.Id;
                repoInfos.Title = repo.Title;
                repoInfos.Description = repo.Description;

                List<GithubRepoIssue> issues = await GetGithubAsync<List<GithubRepoIssue>>(
                    string.Format(HTTP_GITHUB_GET_REPOSITORY_ISSUES, publicUsername, projectName)
                );
                repoInfos.Issues = issues;

                List<GithubMilestone> features = await GetGithubAsync<List<GithubMilestone>>(
                    string.Format(HTTP_GITHUB_GET_REPOSITORY_MILESTONES, publicUsername, projectName)
                );
                repoInfos.Features = features;

                return repoInfos;
            }
            catch (Exception e)
            {
                Console.WriteLine($"ReadProjectFromGithubAsync: Error: {e.Message}");
                return null;
            }
        }

        static async Task SyncGithubRepoWithIssuetrackerAsync(FullGithubRepo repo, string issueTrackerServer)
        {

        }

        static async Task<T> GetGithubAsync<T>(string urlWithParams)
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
                Console.WriteLine($"GetGithubAsync: Error: {e.Message}");
                return default;
            }
        }

    }
}
