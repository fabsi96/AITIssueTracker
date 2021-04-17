using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;

namespace AITIssueTracker.API.v0._2_Manager
{
    public class ProjectManager
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

        public async Task<List<ProjectView>> GetProjectsAsync(string filter="")
        {
            List<Project> allProjects = await DbContext.SelectAllAsync();
            return allProjects?.ConvertAll(c => c.AsView());
        }

        public async Task<bool> DeleteProjectAsync(Guid projectId)
        {
            return await DbContext.DeleteByTitleAsync(projectId);
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
}