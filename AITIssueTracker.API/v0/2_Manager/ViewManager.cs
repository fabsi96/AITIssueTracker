using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;

namespace AITIssueTracker.API.v0._2_Manager
{
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

        public async Task<IssuesView> GetAllFeaturesAndIssuesOfProjectAsync(Guid projectId)
        {
            return await DbContext.SelectIssuesOfProjectAsync(projectId);
        }

        public async Task<IssuesView> GetAllFeaturesAndIssuesOfUserAsync(string username)
        {
            return await DbContext.SelectIssuesOfUserAsync(username);
        }

        public async Task<List<UserView>> GetUsersOfProjectAsync(Guid projectId)
        {
            List<User> allUsers = await DbContext.SelectUsersOfProjectAsync(projectId);
            return allUsers?.ConvertAll(c => c.AsView());
        }

        public async Task<UserIssuesView> GetIssuesOfUserAsync(string username)
        {
            IssuesView issuesViewOfUser = await DbContext.SelectIssuesOfUserAsync(username);
            List<Issue> usersFeatureIssues = new List<Issue>();
            issuesViewOfUser.ProjectFeatures.ForEach(issue =>
            {
                usersFeatureIssues.AddRange(issue.FeatureIssues);
            });

            UserIssuesView userIssuesView = new UserIssuesView
            {
                ProjectIssues = issuesViewOfUser.ProjectIssues,
                FeatureIssues = usersFeatureIssues,
            };

            return userIssuesView;
        }
    }
}