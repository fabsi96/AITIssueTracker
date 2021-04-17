using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;

namespace AITIssueTracker.API.v0._2_Manager
{
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

        public async Task<IssueView> EditIssueAsync(IssueForm issue, Guid issueId, string username)
        {
            Issue issueToUpdate = new Issue(issue) { Id = issueId };
            Issue originalIssue = await DbContext.SelectIssueAsync(issueId);
            List<User> issueUsers = await DbContext.SelectUsersOfIssueAsync(issueId);

            if (originalIssue is null && !issueUsers.Exists(u => u.Username == username))
                return null;

            return await DbContext.UpdateIssueAsync(issueToUpdate) == 1 ? issueToUpdate.AsView() : null;
        }
        public async Task<IssueView> EditIssueTypeAsync(IssueType newIssueType, Guid issueId, string username)
        {
            Issue originalIssue = await DbContext.SelectIssueAsync(issueId);
            List<User> issueUsers = await DbContext.SelectUsersOfIssueAsync(issueId);

            if (originalIssue is null || 
                !issueUsers.Exists(u => u.Username == username))
                return null;

            if (originalIssue.Type == newIssueType)
                return originalIssue.AsView();

            originalIssue.Type = newIssueType;
            return await DbContext.UpdateIssueAsync(originalIssue) == 1 ? originalIssue.AsView() : null;
        }

        public async Task<IssueView> EditIssueStatusAsync(FeatureStatus newIssueStatus, Guid issueId, string username)
        {
            Issue originalIssue = await DbContext.SelectIssueAsync(issueId);
            List<User> issueUsers = await DbContext.SelectUsersOfIssueAsync(issueId);

            if (originalIssue is null ||
                !issueUsers.Exists(u => u.Username == username))
                return null;

            if (originalIssue.Status == newIssueStatus)
                return originalIssue.AsView();

            originalIssue.Status = newIssueStatus;
            return await DbContext.UpdateIssueAsync(originalIssue) == 1 ? originalIssue.AsView() : null;
        }

        public async Task<bool> DeleteIssueFromProjectAsync(Guid issueId)
        {
            return await DbContext.DeleteIssueFromProjectById(issueId) == 1;
        }

        public async Task<bool> DeleteIssueFromFeatureAsync(Guid issueId)
        {
            return await DbContext.DeleteIssueFromFeatureById(issueId) == 1;
        }

        /* === Extended user functions ==== */

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
}