using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._3_ViewModel;

namespace AITIssueTracker.API.v0._2_Manager.Contracts
{
    public interface IProjectService
    {
        Task<ProjectView> SaveProjectAsync(ProjectForm projectForm);

        Task<List<ProjectView>> GetProjectsAsync(string filter = "");

        Task<bool> DeleteProjectAsync(Guid projectId);

        Task<bool> AddUserToProjectAsync(string username, Guid projectId);

        Task<bool> RemoveUserFromProjectAsync(string username, Guid projectId);
    }
}